// ============================================================================
// NeuroForge
// File: MockHttpClientHelper.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Helper class for creating mock HttpClient and HttpMessageHandler
//
// License: MIT
// ============================================================================
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ============================================================================

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