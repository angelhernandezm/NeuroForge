using Moq;
using Moq.Protected;
using System.Net;

namespace NeuroForge.Testing.Helpers;

/// <summary>
/// Helper class for creating mock HttpClient and HttpMessageHandler
/// </summary>
public static class MockHttpClientHelper {
    /// <summary>
    /// Creates a mock HttpMessageHandler that returns the specified response
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="content">The content.</param>
    /// <returns>Mock{HttpMessageHandler}.</returns>
    public static Mock<HttpMessageHandler> CreateMockMessageHandler(
        HttpStatusCode statusCode,
        HttpContent? content = null) {
        var mockHandler = new Mock<HttpMessageHandler>();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = statusCode,
                Content = content ?? new StringContent(string.Empty)
            });

        return mockHandler;
    }

    /// <summary>
    /// Creates a mock HttpMessageHandler that returns a success response with byte content
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="contentLength">Length of the content.</param>
    /// <returns>Mock{HttpMessageHandler}.</returns>
    public static Mock<HttpMessageHandler> CreateMockMessageHandlerWithBytes(
        byte[] content,
        long? contentLength = null) {
        var mockHandler = new Mock<HttpMessageHandler>();

        var byteContent = new ByteArrayContent(content);
        if (contentLength.HasValue) {
            byteContent.Headers.ContentLength = contentLength.Value;
        }

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = byteContent
            });

        return mockHandler;
    }

    /// <summary>
    /// Creates a mock HttpMessageHandler that throws an exception
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>Mock{HttpMessageHandler}.</returns>
    public static Mock<HttpMessageHandler> CreateMockMessageHandlerWithException(
        Exception exception) {
        var mockHandler = new Mock<HttpMessageHandler>();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        return mockHandler;
    }

    /// <summary>
    /// Creates a mock HttpClient from a mock HttpMessageHandler
    /// </summary>
    /// <param name="mockHandler">The mock handler.</param>
    /// <returns>HttpClient.</returns>
    public static HttpClient CreateMockHttpClient(Mock<HttpMessageHandler> mockHandler) {
        return new HttpClient(mockHandler.Object);
    }
}