
const { DataApi } = require("@unity-services/cloud-save-1.0");
const { InventoryApi } = require("@unity-services/economy-2.4");


module.exports = async ({ params, context, logger }) => {
  try {
    const { projectId, playerId, accessToken } = context;

    const inventoryApi = new InventoryApi({ accessToken });

    //get inventory items
    const inventoryRequest = { playerId, projectId };
    const inventoryResponse = await inventoryApi.getPlayerInventory(inventoryRequest);
    const items = inventoryResponse.data.results;

    const isCharacterExist = items.some(item => item.inventoryItemId === params.characterId.toUpperCase());

    //check character exist
    if(!isCharacterExist) 
      throw new Error("You do not own this character.");
    
    const cloudCodeApi = new DataApi(context);
    await cloudCodeApi.setItem(projectId, playerId, { key: "characterInUse", value: params.characterId })



    return {
      isSuccess: true,
      message: "Change character success!",
      data: params.characterId
    };
  } catch (error) {
    transformAndThrowCaughtError(error);
  }
};

function transformAndThrowCaughtError(error) {
  let result = {
    isSuccess: false,
    message: "",
    data : null
  };

  if (error.response) {
    result.message = error.response.data.detail ? error.response.data.detail : error.response.data;
  } else {
    result.message = error.message
  }
  throw new Error(JSON.stringify(result));
}