/* Redux store actions */

const loginUser = (googleAuthTokenData) => {
    return (
        {
            type: "LOGIN_USER",
            payload: googleAuthTokenData
        }
    )
}

const logoffUser = () => {
    return (
        {
            type: "LOGOFF_USER",
            payload: null
        }
    )
}

export { loginUser, logoffUser };