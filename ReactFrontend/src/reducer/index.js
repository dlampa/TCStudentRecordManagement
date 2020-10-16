/* Redux Reducer

Data structure


[ auth: { fullName: string, email: string, idToken: string, rights: string enum (student, staff, super admin), profilePic: string(URL) } ]

*/


const TCDataReducer = (state = [], action) => {
    switch (action.type) {
        case "LOGIN_USER":
            const returnState = state.map(authObject => {
                
            });
            return returnState;
        case "LOGOFF_USER":
            break;
        default:
            return state;
    }
}

export default TCDataReducer;