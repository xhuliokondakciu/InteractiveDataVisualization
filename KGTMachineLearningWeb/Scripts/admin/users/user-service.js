class UserService {

    _onError(errorMessage,error) {
        console.error(errorMessage,error);
    }

    sendDeleteUserRequest(userId) {
        return $.ajax({
            url: `${usersAPI.deleteUserUrl}/${userId}`,
            type: 'GET',
            error: (error) => {
                this._onError("Couldn't load users", error);
            }
        });
    }

    sendGetRolesRequest() {
        return $.ajax({
            url: usersAPI.getRolesUrl,
            type: 'GET',
            error: (error) => {
                this._onError("Couldn't get roles", error);
            }
        });
    }

    sendGetCreateUserViewRequest() {
        return $.ajax({
            url: usersAPI.createUserUrl,
            type: "GET",
            dataType: "html",
            error: (error) => {
                this._onError(error);
            }
        });
    }

    sendCreateUserRequest(userData) {
        return $.ajax({
            url: usersAPI.createUserUrl,
            type: 'POST',
            data: userData,
            dataType:"html",
            error: (error) => {
                this._onError("Couldn't get roles", error);
            }
        })
    }
}