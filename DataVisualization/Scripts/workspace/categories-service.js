class CategoriesService {

    _onError(errorMessage, error) {
        console.error(errorMessage, error);
    }

    _pushHistoryState(url, categoryId, viewType) {
        history.pushState({
            categoryId: categoryId,
            viewType: viewType
        }, document.title, url);
    }

    /**
     * Send a create category request
     * @param {any} parentId Parent category id where to create the new category
     * @param {any} newTitle Title of the new category
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendCreateCategoryRequest(parentId, newTitle) {
        return $.ajax({
            url: workspaceApi.createCategoryUrl,
            method: "POST",
            dataType: "json",
            data: { parentId: parentId, categoryTitle: newTitle },
            error: function(error) {
                this._onError("Category couldn't be created", error);
            }
        });
    }

    /**
     * Send a rename category reqeust
     * @param {any} categoryId Category to rename id
     * @param {any} newTitle New title
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendRenameCategoryRequest(categoryId, newTitle) {
        return $.ajax({
            url: workspaceApi.renameCategoryUrl,
            method: 'post',
            data: { categoryId: categoryId, newTitle: newTitle },
            error: function (error) {
                this._onError("Couldn't delete category", error);
            }
        });
    }

    /**
     * Send a request to delete category
     * @param {any} categoryId Category to delete id
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendDeleteCategoryRequest(categoryId) {
        return $.ajax({
            url: workspaceApi.deleteCategoryUrl,
            method: 'post',
            data: { categoryId: categoryId },
            error: function (error) {
                this._onError("Couldn't delete category", error);
            }
        });
    }

    /**
     * Send a request to rename the chart objects
     * @param {any} chartObjectId Id of the chart object to rename
     * @param {any} newTitle New title
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendRenameChartObjectRequest(chartObjectId, newTitle) {
        return $.ajax({
            url: workspaceApi.renameChartObjectUrl,
            method: 'post',
            data: { chartObjectId: chartObjectId, newTitle: newTitle },
            error: function (error) {
                this._onError("Couldn't delete category", error);
            }
        });
    }

    /**
     * Send a request to change the chart object category
     * @param {any} chartObjectId Chart object id to change category
     * @param {any} newCategoryId The new cateory of the chart object
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendChangeChartObjectCategoryRequest(chartObjectId, newCategoryId) {
        return $.ajax({
            url: workspaceApi.changeChartObjectCategoryUrl,
            method: 'POST',
            data: { chartObjectId: chartObjectId, newCategoryId: newCategoryId },
            error: function (error) {
                this._onError("Couldn't move chart object", error);
            }
        });
    }

    /**
     * Send a request to move the specified category to another category
     * @param {any} categoryId Category id to move
     * @param {any} newParentId Category id where to move the category
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendMoveCategoryRequest(categoryId, newParentId) {
        return $.ajax({
            url: workspaceApi.moveCategoryUrl,
            method: 'post',
            data: { categoryId: categoryId, newParentId: newParentId },
            error: function (error) {
                this._onError("Couldn't move category", error);
            }
        });
    }

    /**
     * Send a request to delete chart object
     * @param {any} chartObjectId Chart object id to delete
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendDeleteChartObjectRequest(chartObjectId) {
        return $.ajax({
            url: workspaceApi.deleteChartObjectUrl,
            method: 'post',
            data: { chartObjectId: chartObjectId },
            error: function (error) {
                this._onError("Couldn't delete chart object", error);
            }
        });
    }

    /**
     * Send a request to get contents of specified category
     * @param {any} categoryId Category to get contents for
     * @param {any} viewType View type of the returned result, can be list or grid
     * @param {any} fromPopState Is the request from history pop state
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendGetCategoryContentsRequest(categoryId, viewType, fromPopState = false) {

        if (this.currentGetCategoryRequest && this.currentGetCategoryRequest.state() === "pending") {
            this.currentGetCategoryRequest.abort();
        }

        const url = `${workspaceApi.categoryContentsUrl}/${categoryId}?viewType=${viewType}`;
        this.currentGetCategoryRequest =  $.ajax({
            url: url,
            method: 'get',
            dataType: 'html',
            success: () => {
                if (!fromPopState)
                    this._pushHistoryState(url, categoryId, viewType);
            },
            error: (error) => {
                if (!error.statusText === "abort") {
                    this._onError("Couldn't load category contents", error);
                }
                
            }
        });
        return this.currentGetCategoryRequest;
    }

    /**
     * Send a request to search categories and chart objects
     * @param {any} searchTerm Search term
     * @param {any} viewType View type of the returned result, can be list or grid
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendSearchCategoriesRequest(searchTerm, viewType) {
        return $.ajax({
            url: `${workspaceApi.searchCategoriesUrl}?searchTerm=${searchTerm}&viewType=${viewType}`,
            method: 'get',
            dataType: 'html',
            error: (error) => {
                this._onError("Couldn't complete search", error);
            }
        });
    }

    /**
     * Send a request to edit category description
     * @param {any} categoryId Id of category to edit the description
     * @param {any} newDescription Description value
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendEditCategoryDescriptionRequest(categoryId, newDescription) {
        return $.ajax({
            type: "POST",
            url: workspaceApi.editCategoryDescriptionUrl,
            data: { categoryId: categoryId, newDescription: newDescription },
            error: (error) => {
                this._onError("Couldn't edit category description", error);
            }
        });
    }

    /**
     * Send a request to edit chart object description
     * @param {any} chartObjectId Id of chart object to edit the description
     * @param {any} newDescription Description value
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendEditChartObjectDescriptionRequest(chartObjectId, newDescription) {
        return $.ajax({
            type: "POST",
            url: workspaceApi.editChartObjectDescriptionUrl,
            data: { chartObjectId: chartObjectId, newDescription: newDescription },
            error: (error) => {
                this._onError("Couldn't edit chart object description", error);
            }
        });
    }

    /**
     * Send a request to copy category
     * @param {any} categoryToCopyId Id of category to copy
     * @param {any} parentCategoryId Id of category where to copy the category
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendCopyCategoryRequest(categoryToCopyId, parentCategoryId) {
        return $.ajax({
            type: "POST",
            url: workspaceApi.copyCategoryUrl,
            data: { categoryToCopyId: categoryToCopyId, parentCategoryId: parentCategoryId },
            error: (error) => {
                this._onError("Couldn't copy category", error);
            }
        });
    }

    /**
     * Send a request to copy chart object
     * @param {any} chartObjectId Id of chart object to copy
     * @param {any} parentCategoryId Id of category where to copy chart object
     * @returns {JQuery.jqXHR} Request jqXHR object
     */
    sendCopyChartObjectRequest(chartObjectId, parentCategoryId) {
        return $.ajax({
            type: "POST",
            url: workspaceApi.copyChartObjectUrl,
            data: { chartObjectId: chartObjectId, targetCategoryId: parentCategoryId },
            error: (error) => {
                this._onError("Couldn't copy chart object", error);
            }
        });
    }

    sendCreateChartObjectRequest(title, description, categoryId) {
        return $.ajax({
            type: "POST",
            url: workspaceApi.createChartObjectUrl,
            data: { categoryId, description, title },
            error: (error) => {
                this._onError("Couldn't create chart object", error);
            }
        });
    }


}