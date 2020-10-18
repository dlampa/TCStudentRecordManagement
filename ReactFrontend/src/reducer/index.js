/* Redux Reducer

Data structure


{ 
    auth: 
    { 
        fullName: string, 
        email: string, 
        tokenID: string, 
        rights: string enum (student, staff, super admin), 
        cohortID: int
        profilePic: string(URL) 
    } 
}

*/


const TCDataReducer = (state = [], action) => {
    switch (action.type) {
        case "LOGIN_USER":
            const returnLoginState = {
                auth: {
                    fullName: action.payload.data.fullname,
                    email: action.payload.data.email,
                    rights: action.payload.data.groupMembership,
                    tokenID: action.payload.data.tokenID,
                    imageURL: action.payload.data.imageURL,
                    cohortID: action.payload.data.cohortID
                }
            }
            return returnLoginState;
        case "LOGOFF_USER":
            const returnLogoffState = {
                auth: {
                    fullName: null,
                    email: null,
                    rights: null,
                    tokenID: null,
                    imageURL: null
                }
            }
            return returnLogoffState;
        default:
            return state;
    }
}

export default TCDataReducer;