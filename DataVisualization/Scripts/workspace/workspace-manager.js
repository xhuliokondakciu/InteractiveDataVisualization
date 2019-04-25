class WorkspaceManager {
    /**
     * Construct WorkspaceService object
     * @param {CategoriesService} categoriesService Category service instance
     * @param {string} treeContainerSelector jQuery selector for tree container
     * @param {string} sharedTreeContainerSelector jQuery selector for shared tree container
     * @param {string} categoryContentsContainerSelector jQuery selector for category contents container
     * @param {string} viewType Type of view for the category contents. It can be grid or list
     * @param {object} hierarchyToOpen Tree hierarchy to open on load
     * @returns {void}
     */
    constructor(
        categoriesService,
        treeContainerSelector,
        sharedTreeContainerSelector,
        categoryContentsContainerSelector,
        viewType,
        hierarchyToOpen) {
        this.categoriesService = categoriesService;
        this.treeContainerSelector = treeContainerSelector;
        this.categoryContentsContainerSelector = categoryContentsContainerSelector;
        this.sharedTreeContainerSelector = sharedTreeContainerSelector;
        this.viewType = viewType;
        this.hierarchyToOpen = hierarchyToOpen;
        this.createChartModalSelector = "#create-chart-object-modal";
        this.createChartFormSelector = "#create-chart-form";

        //Upload file selectors
        this.uploadChartDataModalSelector = "#upload-chart-data-modal";
        this.uploadChartDataFormSelector = "#upload-chart-data-form";
        this.chartTitleFileUploadSelector = "#chart-title-file-input";
    }



    initWorkspace() {
        this.treeManager = new TreeManager(this.treeContainerSelector, this.sharedTreeContainerSelector, this.categoriesService, this.hierarchyToOpen);
        this.categoriesContentManager = new CategoriesContentManager(
            this.viewType,
            this.categoryContentsContainerSelector,
            this.categoriesService,
            this.treeContainerSelector,
            this.sharedTreeContainerSelector);


        this.treeManager.initTree();
        this.categoriesContentManager.intiCategoriesContent();

        this.onLoadCategoryContent();
        this.onNodeActivated();
        this.fixToBottom();
        this.registerOnPopState();
        this.initCreateChartForm();
       
        //Upload file init
        this.uploadFileToProcessInit();
        this.initUploadChartDataForm();
    }

    onLoadCategoryContent() {
        document.addEventListener("loadCategoryContents", (event) => {
            this.categoriesContentManager.loadCategoryContent(event.detail.categoryId);
        });
    }

    onNodeActivated() {
        document.addEventListener("nodeActivated", (event) => {
            this.categoriesContentManager.loadCategoryContent(event.detail.categoryId);
            this.categoriesContentManager.clearSearch();
        });
    }

    fixToBottom() {
        $("#categoriesTreeColumn,#categoriesContentColumn").fixToBottom(5);
    }

    uploadFileToProcessInit() {

        $("#upload-file").click(() => {
            let node = this.treeManager.activeNode;

            if (!node) {
                alert("Please select a category.");
                return false;
            }

            if (!node.data.allowedActions.some(a => a === KGT.Constants.permissions.create)) {
                alert("You can't upload to this category");
                return false;
            }
            $("#upload-category-name").attr("value", node.title);
            $(this.uploadChartDataFormSelector).trigger("reset");
            $(this.uploadChartDataFormSelector).find("#chart-file-category-id").val(node.key);
            $(this.uploadChartDataModalSelector).modal('show');
        });
        
    }

    registerOnPopState() {
        window.onpopstate = (event) => {
            const state = event.state;
            this.categoriesContentManager.viewType = state.viewType;
            this.categoriesContentManager.changeViewTypeButtonStyle();
            this.categoriesContentManager.loadCategoryContent(event.state.categoryId, true);
            this.treeManager.setNodeActive(state.categoryId, true);
        };
    }

    initCreateChartForm() {
        const $form = $(this.createChartFormSelector);
        $form.validate({
            errorClass: "is-invalid text-danger"
        });
        $form.submit((e) => {
            e.preventDefault();
            if (!$form.valid()) return;
            const chartTitle = $("#chart-title-input").val();
            const chartDescription = $("#chart-description-input").val();
            const categoryId = $("#category-id-input").val();
            this.categoriesService.sendCreateChartObjectRequest(chartTitle, chartDescription, categoryId)
                .done(() => {
                    if (this.treeManager.activeNode && this.treeManager.activeNode.key == categoryId)
                        this.categoriesContentManager.loadCategoryContent();

                    $(this.createChartFormSelector).trigger('reset');
                    $(this.createChartModalSelector).modal('hide');
                });
        });
    }

    trackUploadProgress(e,prgBar) {
        
        if (e.lengthComputable) {
            this.currentProgress = Math.floor((e.loaded / e.total) * 100); // Amount uploaded in percent
            prgBar.attr("aria-valuenow", this.currentProgress);
            prgBar.css("width", this.currentProgress + "%");
            prgBar.text(this.currentProgress + "%");

            if (this.currentProgress === 100)
                console.log('Progress : 100%');
        }
    }

    initUploadChartDataForm() {
        let $form = $(this.uploadChartDataFormSelector);
        let validator = $form.validate({
            errorClass: "is-invalid text-danger"
        });
        //Put label of input group
        $('#chart-file-input').on('change', function () {
            if (this.files && this.files.length > 0) {
                $(this).prev('.custom-file-label').html(this.files[0].name);
            }

        });
        //Clear label of input group
        $(this.uploadChartDataModalSelector).on("shown.bs.modal", () => {
            //$(this.uploadChartDataFormSelector)[0].reset();
            $(this.uploadChartDataFormSelector).find(".custom-file-label").empty();
        });

        $(this.uploadChartDataFormSelector).submit((e) => {
            e.preventDefault();
            let $form = $(this.uploadChartDataFormSelector);
            if (!$form.valid() || !$form.find("input[type='file']")[0] || $form.find("input[type='file']")[0].files.length === 0) {

                if ($("#chart-file-input-error").length !== 0) return;

                $(this.uploadChartDataFormSelector).find(".custom-file-label").addClass("text-danger");
                $(this.uploadChartDataFormSelector).find(".input-group").css("border", "1px solid #dc3545");
                let errorLabel = $("<label />", {
                    id: "chart-file-input-error",
                    class: "isinvalid text-danger",
                    for: "chart-file-input",
                    text: "This field is required."
                });

                $(this.uploadChartDataFormSelector).find(".input-group").after(errorLabel);

                $(this.uploadChartDataModalSelector).on("hidden.bs.modal", () => {
                    errorLabel.remove();
                    $(this.uploadChartDataFormSelector).find(".custom-file-label").removeClass("text-danger");
                    $(this.uploadChartDataFormSelector).find(".input-group").css("border", "");
                    $("#chart-title-file-input").removeClass("is-invalid text-danger");
                    validator.resetForm();
                });

                return;
            }

            let file = $form.find("input[type='file']")[0].files[0];
            const categoryId = $("#chart-file-category-id").val();
            const chartTitle = $(this.chartTitleFileUploadSelector).val();
            const configId = $("#chartConfig").val();
            if (window.FormData !== undefined) {
                let data = new FormData();
                data.append("file", file, file.name);
                data.append("categoryId", categoryId);
                data.append("chartTitle", chartTitle);
                data.append("configId", configId);

                let progressContainer = $("<div/>", { class: "progress mt-2" });
                let progressBar = $("<div/>", {
                    class: "progress-bar progress-bar-striped progress-bar-animated",
                    role: "progressbar",
                    "aria-valuenow": "0",
                    "aria-valuemin": "0",
                    "aria-valuemax": "100"
                });
                progressContainer.append(progressBar);

                $.ajax({
                    type: "post",
                    url: $form.attr("action"),
                    contentType: false,
                    processData: false,
                    data: data,
                    beforeSend: () => {
                        $(this.uploadChartDataModalSelector).find(".modal-body").append(progressContainer);
                    },
                    xhr: () => {
                        var appXhr = $.ajaxSettings.xhr();
                        
                        if (appXhr.upload) {
                            appXhr.upload.addEventListener('progress', (e) => {
                                this.trackUploadProgress(e, progressBar);
                            }, false);
                        }
                        return appXhr;
                    },
                    error: (e) => {
                        console.error("Coulnd't upload file", e);
                        KGT.Helper.showAlert("Couldn't upload file", "alert-danger");
                    },
                    complete: () => {
                        $(this.uploadChartDataFormSelector).trigger('reset');
                        $(this.uploadChartDataModalSelector).modal('hide');
                        progressContainer.remove();
                    }
                });
            } else {
                $form.submit();
            }
        });
    }

}