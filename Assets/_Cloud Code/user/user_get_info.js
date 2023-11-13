const { DataApi } = require("@unity-services/cloud-save-1.0");

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

    return data;

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
    status: 0,
    message: "",
  };

  if (error.response) {
    result.status = error.response.data.status ? error.response.data.status : 0;
    result.message = error.response.data.detail ? error.response.data.detail : error.response.data;
  } else {
    result.status = 400;
    result.message = error.message;
  }
  if (result.message == "Cannot read property 'instanceData' of undefined") result.message = "Please try again! can't find your item in server";

  throw new Error(JSON.stringify(result));
}