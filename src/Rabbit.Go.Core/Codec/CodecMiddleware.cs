using System.Threading.Tasks;

namespace Rabbit.Go.Core
{
    public class CodecMiddleware
    {
        private readonly GoRequestDelegate _next;

        public CodecMiddleware(GoRequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(GoContext context)
        {
            var goFeature = context.Features.Get<IGoFeature>();

            await EncodeAsync(context.Request, goFeature);

            await _next(context);

            await DecodeAsync(context.Response, goFeature);
        }

        private static async Task EncodeAsync(GoRequest request, IGoFeature feature)
        {
            if (feature?.Encoder == null)
                return;
            var instance = feature.RequestType == null ? null : feature.RequestInstance;
            if (instance == null)
                return;

            await feature.Encoder.EncodeAsync(instance, instance.GetType(), request);
        }

        private static async Task DecodeAsync(GoResponse response, IGoFeature feature)
        {
            if (feature.ResponseType == null || feature?.Decoder == null)
                return;

            feature.ResponseInstance = await feature.Decoder.DecodeAsync(response, feature.ResponseType);
        }
    }
}