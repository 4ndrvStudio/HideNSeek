const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.3");
const { CurrenciesApi, InventoryApi } = require("@unity-services/economy-2.4");


module.exports = async ({ params, context, logger }) => {
  try {

    const { projectId, playerId, accessToken } = context;
    const cloudCodeApi = new DataApi(context);
    const inventoryApi = new InventoryApi({ accessToken });

    let initSetup = await cloudCodeApi.getItems(projectId, playerId, ["initSetup"]);

    if (initSetup.data.results.length == 0) {
      await cloudCodeApi.setItem(projectId, playerId, { key: "initSetup", value: true })
      //add character
      const addInventoryRequest = {
        addInventoryRequest: {
          instanceData: {
            type: "character"
          },
          inventoryItemId: "CHARACTER_1",
        },
        playerId,
        projectId,
      };

      const addInventoryResponse = await inventoryApi.addInventoryItem(addInventoryRequest);
      await cloudCodeApi.setItem(projectId, playerId, { key: "characterInUse", value: "character_1" })
      
    }

    return {
      isSuccess: true,
      message: "Setup Complete",
      data: null
    };

  } catch (error) {
    transformAndThrowCaughtError(error);
  }

};




function transformAndThrowCaughtError(error) {
  let result = {
    isSuccess: false,
    message: "",
    data: null
  };

  if (error.response) {
    result.message = error.response.data.detail ? error.response.data.detail : error.response.data;
  } else {
    result.message = error.message
  }
  throw new Error(JSON.stringify(result));
}