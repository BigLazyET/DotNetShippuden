using Microsoft.AspNetCore.Http.Features;

namespace KestrelApp.Common;

/// <summary>
/// 应用请求上下文
/// </summary>
public class ApplicationContext
{
    /// <summary>
    /// 特征合集
    /// </summary>
    public IFeatureCollection Features { get; }

    /// <summary>
    /// 应用程序请求上下文
    /// </summary>
    /// <param name="features"></param>
    public ApplicationContext(IFeatureCollection features)
    {
        Features = new FeatureCollection(features);
    }
}