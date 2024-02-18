const { CurrenciesApi, InventoryApi } = require("@unity-services/economy-2.4");
const { SettingsApi } = require("@unity-services/remote-config-1.0");

module.exports = async ({ params, context, logger }) => {
    try {
        const { projectId, playerId, accessToken, environmentId } = context;
        const currencyApi = new CurrenciesApi({ accessToken });
        const inventoryApi = new InventoryApi({ accessToken });
        const settingApi = new SettingsApi({ accessToken });

        const currencyRequest = { projectId, playerId };
        const currencyRespone = await currencyApi.getPlayerCurrencies(currencyRequest);

        //get current gold
        const currentGem = currencyRespone.data.results.find(currency => currency.currencyId == "GEM");

        //get config
        const configType = "settings";
        const key = ["bundlePackConfig"];
        const settingConfigRespone = await settingApi.assignSettingsGet(projectId, environmentId, configType, key);
        const bundlePackConfig = settingConfigRespone.data.configs.settings["bundlePackConfig"];
        const characterPack = bundlePackConfig.character.find(character => character.id == params.characterId);

        //Validate State
        if (currentGem.balance < characterPack.price) throw new Error("Your not enough GEM");

        //character
        const addInventoryRequest = {
            addInventoryRequest: {
                instanceData: {
                    type: "character"
                },
                inventoryItemId: params.characterId.toUpperCase(),
            },
            playerId,
            projectId,
        };

        const addInventoryResponse = await inventoryApi.addInventoryItem(addInventoryRequest);

        //request update Gem 
        const gemRequest = {
            currencyId: "GEM",
            currencyModifyBalanceRequest: {
                amount: characterPack.price
            },
            playerId,
            projectId
        }

        let currencyResult = await currencyApi.decrementPlayerCurrencyBalance(gemRequest);


        return {
            isSuccess: true,
            message: `Purchased successfully.`,
            data: null
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
        result.message = error.response.data.detail ? error.response.data.detail.replace('currency', 'Gold') : error.response.data;
    } else {
        result.message = error.message;
    }

    throw new Error(JSON.stringify(result));
}
