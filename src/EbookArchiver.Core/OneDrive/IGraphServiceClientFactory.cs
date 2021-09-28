using Microsoft.Graph;

namespace EbookArchiver.OneDrive
{
    public interface IGraphServiceClientFactory
    {
        GraphServiceClient GetGraphClientForScopes(params string[] scopes);
    }
}
