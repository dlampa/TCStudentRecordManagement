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
            const returnState = {
                auth: {
                    fullName: action.payload.data.fullname,
                    email: action.payload.data.email,
                    rights: action.payload.data.groupMembership,
                    tokenID: action.payload.data.tokenID,
                    imageURL: action.payload.data.imageURL
                }
            }
            return returnState;
        case "LOGOFF_USER":
            break;
        default:
            return state;
    }
}

export default TCDataReducer;