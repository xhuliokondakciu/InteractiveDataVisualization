using Highsoft.Web.Mvc.Charts;

namespace DataVisualization.Domain.Contracts
{
    public interface INeuralNetworkDomain
    {
        Highcharts GetNeuralNetworkOptions();
    }
}