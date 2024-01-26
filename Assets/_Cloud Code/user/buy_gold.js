const { CurrenciesApi } = require("@unity-services/economy-2.4");
const { SettingsApi } = require("@unity-services/remote-config-1.0");

module.exports = async ({ params, context, logger }) => {
  
  try {
    const { projectId, playerId, accessToken, environmentId } = context;
    const currencyApi = new CurrenciesApi({ accessToken });
    const settingApi = new SettingsApi({ accessToken });

    const currencyRequest = { projectId, playerId };
    const currencyRespone = await currencyApi.getPlayerCurrencies(currencyRequest);

    //get current gem
    const currentGem = currencyRespone.data.results.find(currency => currency.currencyId == "GEM");

    //get config
    const configType = "settings";
    const key = ["BundlePackConfig"];
    const settingConfigRespone = await settingApi.assignSettingsGet(projectId, environmentId, configType, key);
    const bundlePackConfig = settingConfigRespone.data.configs.settings["BundlePackConfig"];
    const pack = bundlePackConfig.gold.find(pack => pack.packId == params.bundlePackId);
   
    //Validate State
    if(currentGem < pack.price) throw new Error("Your not enough GEM");


    //request to update 
    const goldRequest = {
      currencyId: "GOLD",
      currencyModifyBalanceRequest: {
        amount: pack.amount
      },
      playerId,
      projectId
    }
    const gemRequest = {
      currencyId: "GEM",
      currencyModifyBalanceRequest: {
        amount: pack.price
      },
      playerId,
      projectId
    }


    let currencyResult = await currencyApi.incrementPlayerCurrencyBalance(goldRequest);
    let minusGem  = await currencyApi.decrementPlayerCurrencyBalance(gemRequest);

    return {
      isSuccess : true,
      message: `You claim ${pack.amount} Gold`,
      data: pack.amount
    }


  } catch (err) {
    transformAndThrowCaughtError(err);
  }


};

function transformAndThrowCaughtError(error) {
  let result = {
    isSuccess: false,
    message: "",
    data: null
  };

  if (error.response) {
    result.message = error.response.data.detail ? error.response.data.detail.replace('currency', 'GEM') : error.response.data;
  } else {
    result.message = error.message;
  }

  throw new Error(JSON.stringify(result));
}
