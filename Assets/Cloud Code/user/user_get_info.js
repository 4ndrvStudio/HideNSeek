const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save");


module.exports = async ({ params, context, logger }) => {
  const { projectId, playerId, accessToken } = context;
  const cloudSaveApi = new DataApi({accessToken});

  try {
    await cloudSaveApi.setItem(projectId, playerId, {
      key: "test",
      value: "value"
    });

    result = await cloudSaveApi.getItems(projectId, playerId);

    return result.data;
  } catch (err) {
    logger.error("Error while calling out to Cloud Save", {"error.message": err.message});
    throw err;
  }

};