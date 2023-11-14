const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.0");


module.exports = async ({ params, context, logger }) => {
  try {
    const { projectId, playerId, accessToken } = context;
    const cloudCodeApi = new DataApi(context);
    await cloudCodeApi.setItem(projectId, playerId, { key: "userName", value: params.name })

    return {
      isSuccess: true,
      message: "Change name Success!",
      data: params.name
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