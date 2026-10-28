using Danorjuela.Utils.ResponseProcessor.DTOs;
using Danorjuela.Utils.ResponseProcessor.Exceptions;
using Danorjuela.Utils.ResponseProcessor.Interface;
using Danorjuela.Utils.ResponseProcessor.Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Danorjuela.Utils.ResponseProcessor.Test;
[TestClass]
public class ResponseServiceTest
{

    readonly IServiceProvider _services = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
       .ConfigureServices((hostContext, services) =>
       {
           services.AddResponseProcessorService();

       }).Build().Services;

    private IResponseService Service => _services.GetRequiredService<IResponseService>();

    private static readonly TestResponse Data = new() { Property1 = 1, Property2 = "value" };

    public static IEnumerable<object[]> ExceptionCases =>
    [
        // Excepciones propias del paquete
        [new BadRequestException("error"), 400],
        [new NoDataException("error"), 204],
        [new NotAutorizationException("error"), 401],
        [new ForbiddenException("error"), 403],
        [new NotFoundException("error"), 404],
        [new ConflicException("error"), 409],
        [new GoneException("error"), 410],
        [new PreconditionFailedException("error"), 412],
        [new BusinessException("error"), 418],
        [new UnprocessableEntityException("error"), 422],
        [new TooManyRequestsException("error"), 429],
        [new ServiceUnavailableException("error"), 503],
        // Excepciones estandar de .NET
        [new InvalidOperationException("error"), 400],
        [new ObjectDisposedException("error"), 400],
        [new ArgumentException("error"), 400],
        [new ArgumentNullException("param"), 400],
        [new ArgumentOutOfRangeException("param"), 400],
        [new UnauthorizedAccessException("error"), 403],
        [new KeyNotFoundException("error"), 404],
        [new NotImplementedException("error"), 501],
        [new TimeoutException("error"), 504],
        // No mapeadas
        [new Exception("error"), 500],
        [new NullReferenceException("error"), 500],
        [new DivideByZeroException("error"), 500],
    ];

    public static IEnumerable<object[]> GenericExceptionCases =>
    [
        [new BadRequestException<TestResponse>("error", Data), 400],
        [new NoDataException<TestResponse>("error", Data), 204],
        [new NotAutorizationException<TestResponse>("error", Data), 401],
        [new ForbiddenException<TestResponse>("error", Data), 403],
        [new NotFoundException<TestResponse>("error", Data), 404],
        [new ConflicException<TestResponse>("error", Data), 409],
        [new GoneException<TestResponse>("error", Data), 410],
        [new PreconditionFailedException<TestResponse>("error", Data), 412],
        [new BusinessException<TestResponse>("error", Data), 418],
        [new UnprocessableEntityException<TestResponse>("error", Data), 422],
        [new TooManyRequestsException<TestResponse>("error", Data), 429],
        [new ServiceUnavailableException<TestResponse>("error", Data), 503],
    ];

    [TestMethod]
    public void AddResponseProcessorService_RegistersService()
    {
        var service = _services.GetService<IResponseService>();

        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void AddResponseProcessorService_RegistersAsTransient()
    {
        var first = _services.GetRequiredService<IResponseService>();
        var second = _services.GetRequiredService<IResponseService>();

        Assert.AreNotSame(first, second);
    }

    [TestMethod]
    [DynamicData(nameof(ExceptionCases))]
    public void ProcessException_ReturnsExpectedStatus(Exception ex, int expectedStatus)
    {
        var result = Service.ProcessException(ex) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedStatus, result.StatusCode);
        var body = result.Value as ResponseDTO;
        Assert.IsNotNull(body);
        Assert.AreEqual(ex.Message, body.Message);
        Assert.IsFalse(body.IsSuccess);
        Assert.IsFalse(string.IsNullOrEmpty(body.Guid));
    }

    [TestMethod]
    [DynamicData(nameof(GenericExceptionCases))]
    public void ProcessException_GenericException_ReturnsStatusAndData(Exception ex, int expectedStatus)
    {
        var result = Service.ProcessException(ex) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedStatus, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse>;
        Assert.IsNotNull(body);
        Assert.AreEqual("error", body.Message);
        Assert.AreSame(Data, body.Data);
        Assert.IsFalse(string.IsNullOrEmpty(body.Guid));
    }

    [TestMethod]
    public void ProcessException_GenericException_WithNullData()
    {
        var result = Service.ProcessException(new NotFoundException<TestResponse?>("error", null)) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse?>;
        Assert.IsNotNull(body);
        Assert.IsNull(body.Data);
    }

    [TestMethod]
    public void ProcessException_GenericException_WithValueType()
    {
        var result = Service.ProcessException(new ConflicException<int>("error", 42)) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(409, result.StatusCode);
        var body = result.Value as ResponseDTO<int>;
        Assert.IsNotNull(body);
        Assert.AreEqual(42, body.Data);
    }

    [TestMethod]
    public void ProcessException_GeneratesUniqueGuid()
    {
        var first = (Service.ProcessException(new NotFoundException("error")) as ObjectResult)?.Value as ResponseDTO;
        var second = (Service.ProcessException(new NotFoundException("error")) as ObjectResult)?.Value as ResponseDTO;

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.AreNotEqual(first.Guid, second.Guid);
    }

    [TestMethod]
    public void ProcessException_WithCode_OverridesStatus()
    {
        var result = Service.ProcessException(new NotFoundException("Not Found Element"), 400) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var body = result.Value as ResponseDTO;
        Assert.IsNotNull(body);
        Assert.AreEqual("Not Found Element", body.Message);
        Assert.IsFalse(string.IsNullOrEmpty(body.Guid));
    }

    [TestMethod]
    public void ProcessException_WithCode_GenericException_KeepsData()
    {
        var result = Service.ProcessException(new BusinessException<TestResponse>("error", Data), 400) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse>;
        Assert.IsNotNull(body);
        Assert.AreSame(Data, body.Data);
        Assert.IsFalse(string.IsNullOrEmpty(body.Guid));
    }

    [TestMethod]
    public void ProcessException_WithNullCode_UsesDefaultMapping()
    {
        var result = Service.ProcessException(new TooManyRequestsException("error"), null) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(429, result.StatusCode);
    }

    [TestMethod]
    public void ProcessResponse_DefaultStatusIs200()
    {
        var result = Service.ProcessResponse(Data) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse>;
        Assert.IsNotNull(body);
        Assert.AreSame(Data, body.Data);
        Assert.AreEqual("", body.Message);
        Assert.IsFalse(string.IsNullOrEmpty(body.Guid));
    }

    [TestMethod]
    public void ProcessResponse_WithCode_ReturnsData()
    {
        var result = Service.ProcessResponse(Data, 201) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse>;
        Assert.IsNotNull(body);
        Assert.AreSame(Data, body.Data);
    }

    [TestMethod]
    public void ProcessResponse_WithCollection_ReturnsData()
    {
        var data = new List<TestResponse> { Data };

        var result = Service.ProcessResponse(data) as ObjectResult;

        Assert.IsNotNull(result);
        var body = result.Value as ResponseDTO<List<TestResponse>>;
        Assert.IsNotNull(body);
        Assert.HasCount(1, body.Data!);
    }

    [TestMethod]
    public void ProcessResponse_WithNullData()
    {
        var result = Service.ProcessResponse<TestResponse?>(null) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var body = result.Value as ResponseDTO<TestResponse?>;
        Assert.IsNotNull(body);
        Assert.IsNull(body.Data);
    }


}
