const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.3");

const USER_INFO_KEY = ["userName", "level", "exp"];

const USER_INFO_TEMP = {
  userName: null,
  level: 1,
  exp: 0,
};


module.exports = async ({ params, context, logger }) => {
  try {
    const { projectId, playerId, accessToken } = context;
    const cloudCodeApi = new DataApi(context);
    let userInfoData = await cloudCodeApi.getItems(projectId, playerId, USER_INFO_KEY);
    let data = cloudSaveResponseToObject(userInfoData);

    for (let key in USER_INFO_TEMP) {
      if (!data.hasOwnProperty(key)) {
        await cloudCodeApi.setItem(projectId, playerId, { key: key, value: USER_INFO_TEMP[key] });
        data[key] = USER_INFO_TEMP[key];
      }
    }

    return {
      isSuccess: true,
      message: "",
      data
    };;

  } catch (error) {
    transformAndThrowCaughtError(error);
  }

};

// Functions can exist outside of the script wrapper
function cloudSaveResponseToObject(getItemsResponse) {
  let returnObject = {};
  getItemsResponse.data.results.forEach(item => {
    const key = _.camelCase(item.key);
    returnObject[key] = item.value;
  });

  return returnObject;
}


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