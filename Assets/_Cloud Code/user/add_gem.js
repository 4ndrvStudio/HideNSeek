const { CurrenciesApi } = require("@unity-services/economy-2.4");
 
module.exports = async ({ params, context, logger }) => {

    try {
        const { projectId, playerId, accessToken, environmentId } = context;
        const currencyApi = new CurrenciesApi({ accessToken });

        const currencyRequest = { projectId, playerId };
       
        const gemRequest = {
            currencyId: "GEM",
            currencyModifyBalanceRequest: {
                amount: 10000
            },
            playerId,
            projectId
        }

        let minusGem = await currencyApi.incrementPlayerCurrencyBalance(gemRequest);

        return {
            isSuccess: true,
            message: `You claim ${10000} `,
            data: 10000
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
