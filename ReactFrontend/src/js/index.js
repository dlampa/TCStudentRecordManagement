/* Helper functions for all components */

import axios from 'axios';


const ax = async (apiURL, axConfig, tokenID) => {

    let config = {
        method: 'get',
        url: apiURL,
        headers: { Authorization: `Bearer ${tokenID}` },
        ...axConfig
    };

    try {
        const response = await axios(config);
        return response;
    } catch (error) {
        const err = { errResponse: error?.response?.data, errRequest: error?.request?.message || error?.request?.statusText, message: error?.message };
        console.log(err);
        return err;
    }

}

export { ax };