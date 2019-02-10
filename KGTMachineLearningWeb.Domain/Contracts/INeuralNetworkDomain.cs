using Highsoft.Web.Mvc.Charts;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface INeuralNetworkDomain
    {
        Highcharts GetNeuralNetworkOptions();
    }
}