const { InventoryApi } = require("@unity-services/economy-2.4");
const _ = require("lodash-4.17");

module.exports = async ({ params, context, logger }) => {

  try {
    const { projectId, playerId, accessToken } = context;
    const inventoryApi = new InventoryApi({ accessToken });

    //get inventory items
    const inventoryRequest = { playerId, projectId };
    const inventoryResponse = await inventoryApi.getPlayerInventory(inventoryRequest);
    const items = inventoryResponse.data.results;

    return {
      isSuccess : true,
      message: "Get user characters complete!",
      data: items.filter(item => item.instanceData && item.instanceData.type === "character")
    }
    
  } catch (err) {
    transformAndThrowCaughtError(err);
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