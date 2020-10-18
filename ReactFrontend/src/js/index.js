/* Helper functions for all components */

import axios from 'axios';


const ax = async (apiURL, axConfig, tokenID) => {
    let response;
    let config = {
        method: 'get',
        url: apiURL,
        headers: { Authorization: `Bearer ${tokenID}` },
        ...axConfig
    };

    try {
        response = await axios(config);
    } catch (error) {
        response = error;
    }
    return response;

}

export { ax };