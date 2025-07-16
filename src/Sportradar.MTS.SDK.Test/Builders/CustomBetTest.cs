
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Common.Internal.Rest;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Test.Helpers;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    [DeploymentItem("XML/calculation_response.xml", "XML")]
    [DeploymentItem("XML/available_selections.xml", "XML")]
    [DeploymentItem("XML/response.xml", "XML")]
    public class CustomBetTest
    {
        public static readonly IEnumerable<Tuple<string, string>> UriReplacements = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("/v1/custombet/1222121/available_selections", "XML/available_selections.xml"),
            new Tuple<string, string>("/v1/custombet/calculate", "XML/calculation_response.xml")
        };

        public static readonly IEnumerable<Tuple<string, string>> UriReplacementsError = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("/v1/custombet/calculate", "XML/response.xml")
        };


        ICustomBetManager betManager;
        ICustomBetSelectionBuilder _customSelectionBuilder;

        [TestMethod]
        public void TestAvailableSelections()
        {
            initCustomBet(UriReplacements);
            Task<IAvailableSelections> availSels = betManager.GetAvailableSelectionsAsync("1222121");
            availSels.Wait();
            IAvailableSelections result = availSels.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Markets);

            Dictionary<string, IMarket> mapMarket = fillMarkets(result.Markets);
            Assert.AreEqual(230, mapMarket.Count);
            checkMarket(mapMarket, "90_total=113.5", "12", "13");
            checkMarket(mapMarket, "303_quarternr=2|hcp=-6.5", "1714", "1715");
            checkMarket(mapMarket, "771_variant=pre:playerprops:52630473:607636", "pre:playerprops:52630473:607636:3", "pre:playerprops:52630473:607636:5");
        }

        [TestMethod]
        public void TestCalculationResponse()
        {
            initCustomBet(UriReplacements);
            var selection = _customSelectionBuilder.SetEventId("1222121").SetMarketId(1).SetOutcomeId("12").SetSpecifiers("121").Build();
            List<ISelection> selections = new List<ISelection>();
            selections.Add(selection);

            IEnumerable<ISelection> selectionEnums = selections as IEnumerable<ISelection>;
            Task<ICalculation> calculation = betManager.CalculateProbabilityAsync(selectionEnums);
            calculation.Wait();
            ICalculation result = calculation.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException),"A userId of null was inappropriately allowed.")]
        public void TestCalculationResponseNotPossible()
        {
            initCustomBet(UriReplacementsError);
            var selection = _customSelectionBuilder.SetEventId("1222121").SetMarketId(1).SetOutcomeId("12").SetSpecifiers("121").Build();
            List<ISelection> selections = new List<ISelection>();
            selections.Add(selection);

            IEnumerable<ISelection> selectionEnums = selections as IEnumerable<ISelection>;
            Task<ICalculation> calculation = betManager.CalculateProbabilityAsync(selectionEnums);
            calculation.Wait();
        }

        protected void checkMarket(Dictionary<string, IMarket> mapMarket, string market, params string[] outcomes)
        {
            IMarket marketCheck = mapMarket[market];
            Assert.IsNotNull(marketCheck);
            Assert.IsNotNull(marketCheck.Outcomes);
            List<string> marketOutcomes = getOutcomes(marketCheck.Outcomes);
            foreach (string checkOutcome in outcomes) 
            {
                Assert.IsTrue(marketOutcomes.Contains(checkOutcome));
            }

        }

        protected Dictionary<string, IMarket> fillMarkets(IEnumerable<IMarket> markets)
        {
            Dictionary<string, IMarket> mapMarket = new Dictionary<string, IMarket>();
            foreach (IMarket market in markets)
            {
                string id = market.Id.ToString() + "_" + market.Specifiers;
                mapMarket.Add(id, market);
            }
            return mapMarket;
        }

        protected List<string> getOutcomes(IEnumerable<string>  outcomes)
        {
            List<string> outComesList = new List<string>();
            foreach (string outcome in outcomes)
            {
                outComesList.Add(outcome);
            }
            return outComesList;
        }

        protected void initCustomBet(IEnumerable<Tuple<string, string>> UriReplacements)
        {
            var container = new UnityContainer();
            ISdkConfigurationInternal configInternal = new SdkConfigurationInternal(new SdkConfiguration(SdkConfigurationSectionTest.Create()), null);
            var config = (ISdkConfiguration)configInternal;
            container.RegisterTypes(config, null, null);
            container.RegisterInstance<IDataFetcher>("Base", new DataFetcherHelper(UriReplacements), new ContainerControlledLifetimeManager());
            container.RegisterInstance<IDataPoster>("Base", new DataFetcherHelper(UriReplacements), new ContainerControlledLifetimeManager());

            container.RegisterType<IDeserializer<AvailableSelectionsType>, Deserializer<AvailableSelectionsType>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<AvailableSelectionsType, AvailableSelectionsDTO>, AvailableSelectionsMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<AvailableSelectionsDTO>, DataProvider<AvailableSelectionsType, AvailableSelectionsDTO>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    configInternal.ApiHost + "/v1/custombet/{0}/available_selections",
                    new ResolvedParameter<IDataFetcher>("Base"),
                    new ResolvedParameter<IDataPoster>("Base"),
                    new ResolvedParameter<IDeserializer<AvailableSelectionsType>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<AvailableSelectionsType, AvailableSelectionsDTO>>()));


            container.RegisterType<IDeserializer<CalculationResponseType>, Deserializer<CalculationResponseType>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<CalculationResponseType, CalculationDTO>, CalculationMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICalculateProbabilityProvider, CalculateProbabilityProvider>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    configInternal.ApiHost + "/v1/custombet/calculate",
                    new ResolvedParameter<IDataPoster>("Base"),
                    new ResolvedParameter<IDeserializer<CalculationResponseType>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<CalculationResponseType, CalculationDTO>>()));

            container.RegisterType<ICustomBetManager, CustomBetManager>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IDataProvider<AvailableSelectionsDTO>>(),
                    new ResolvedParameter<ICalculateProbabilityProvider>()));

            betManager = container.Resolve<ICustomBetManager>();
            Assert.IsNotNull(betManager);

            _customSelectionBuilder = betManager.CustomBetSelectionBuilder;
        }

    }
}
;