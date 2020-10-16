/* Redux store actions */

const loginUser = (googleAuthTokenData) => {
    return (
        {
            type: "LOGIN_USER",
            payload: googleAuthTokenData
        }
    )
}

export { loginUser };