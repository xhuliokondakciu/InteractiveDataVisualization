class UserManager {

    /**
     * Create new user manager
     * @param {any} selectors Ui element jquery selectors
     * @param {UserService} userService User service instance
     */
    constructor(
        selectors,
        userService) {
        this.selectors = selectors;
        this.userService = userService;
    }

    init() {
        this.initUserDataTable();
        this.initCreateUser();
    }

    initCreateUser() {
        $(this.selectors.createUserButtonSelector).click(() => {
            //$(this.selectors.createUserModalSelector).modal('show');
            this.userService.sendGetCreateUserViewRequest()
                .done((view) => {
                    this.createUserModal = $(view).modal('show');

                    this.createUserModal.on("shown.bs.modal", () => {
                        $.validator.unobtrusive.parse(this.createUserModal.find("form"));
                        this.initCreateUserForm();
                    });

                    this.createUserModal.on('hidden.bs.modal', () => {
                        this.createUserModal.remove();
                    });
                });
        });
    }

    initCreateUserForm() {
        let $form = this.createUserModal.find('form');
        
        $form.submit((e) => {
            //Prevent submit. Will be handled by ajax
            e.preventDefault();
            const formData = $form.serialize();
            if (!$form.valid()) return;
            this.userService.sendCreateUserRequest(formData)
                .done((view, textStatus, jqXhr ) => {
                    if (jqXhr.status === 201) {//201 -> created
                        this.table.ajax.reload();
                        KGT.Helper.showAlert(jqXhr.statusText,'alert-success');
                    } else {
                        this.createUserModal.on('hidden.bs.modal', () => {
                            this.createUserModal.remove();
                            this.createUserModal = $(view).modal('show');
                            this.createUserModal.on('shown.bs.modal', () => {
                                $.validator.unobtrusive.parse(this.createUserModal.find("form"));
                                this.initCreateUserForm();
                            });

                        });
                    }
                    
                    this.createUserModal.modal('hide');
                });
        });
    }

    initUserDataTable() {
        this.table = $(this.selectors.userTableSelector).DataTable({

            processing: true,
            serverSide: true,
            filter: true,
            orderMulti: false,
            pageLength: 10,
            order:[[1]],
            ajax: {
                url: `${usersAPI.getUsersUrl}`,
                type: "POST",
                datatype: "json"
            },

            columnDefs:
                [{
                    targets: [0],
                    visible: false,
                    searchable: false,
                    orderable:false
                },
                {
                    targets: [1],
                    searchable: true,
                    orderable: true
                },
                {
                    targets: [2],
                    searchable: true,
                    orderable: true
                },
                {
                    targets: [3],
                    searchable: false,
                    orderable: false
                },
                //{
                //    targets: [4],
                //    searchable: false,
                //    orderable: false
                //},
                {
                    targets: [4],
                    searchable: false,
                    orderable: false
                }],

            columns: [
                { data: "id", name: "User id", autoWidth: true },
                { data: "userName", name: "Username", autoWidth: true },
                { data: "email", name: "Email", autoWidth: true },
                {
                    data: "roles",
                    name: "Roles",
                    autoWidth: true,
                    render: (data, type, row) => {
                        return data.join();
                    }
                },
                //{
                //    data: null,
                //    render: (data, type, row) => {
                //        return `<a class="btn btn-outline-info" href="${usersAPI.editUserUrl}/${row.id}">Edit</a>`;
                //    }
                //},
                {
                    data: null,
                    render: (data, type, row) => {

                        return `<a class="btn btn-outline-danger delete-user-button" data-row='${JSON.stringify(row)}'>Delete</a>`;
                    }
                }

            ],
            drawCallback: () => {
                this.registerDeleteUserButtonClick();
            }

        });
    }

    registerDeleteUserButtonClick() {
        $(this.selectors.userTableSelector).find(this.selectors.deleteUserButtonSelector)
            .click((e) => {
            e.stopPropagation();
            e.preventDefault();
            var rowData = $(e.target).data("row");
            if (confirm(`Are you sure that you want to delete user "${rowData.userName}"?`))
                this.deleteUser(rowData.id);
        });
    }

    deleteUser(userId) {
        this.userService.sendDeleteUserRequest(userId)
            .done(() => {
                this.table.ajax.reload(null, false);
            });

    }
}