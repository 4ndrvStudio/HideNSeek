const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.3");


module.exports = async ({ params, context, logger }) => {
  try {
    
    const { projectId, playerId, accessToken } = context;
    const cloudCodeApi = new DataApi(context);
    let userInfoData = await cloudCodeApi.getItems(projectId, playerId, ["characterInUse"]);

    return {
      isSuccess: true,
      message: "Get character complete",
      data: userInfoData.data.results[0].value
    };;

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