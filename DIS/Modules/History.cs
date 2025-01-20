using OpenAI.ObjectModels.RequestModels;
using Qdrant.Client.Grpc;
namespace DIS.Modules;
public class History
{
    Guid ConversationId; 
    List<PointStruct> Points = new List<PointStruct>(); /// FAREMOS UMA COMPARAÇÃO ENTRE OS PONTOS ANTES DE ADICIONAR
    List<ChatMessage> Messages = new List<ChatMessage>(); /// TODAS AS MENSAGENS FICARÃO SALVAS
}