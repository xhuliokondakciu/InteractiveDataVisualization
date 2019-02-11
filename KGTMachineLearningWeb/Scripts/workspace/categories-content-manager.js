class CategoriesContentManager {

    /**
    * @returns {Fancytree.FancytreeNode} Main tree instance
    * */
    get tree() {
        return $.ui.fancytree.getTree(this.treeContainerSelector);
    }

    /**
     * @returns {Fancytree.Fancytree} Shared tree instance
     * */
    get sharedTree() {
        return $.ui.fancytree.getTree(this.sharedTreeContainerSelector);
    }

    /**
     * @returns {Fancytree.FancytreeNode} Current active node
     * */
    get activeNode() {
        return this.tree.getActiveNode() || this.sharedTree.getActiveNode();
    }

    /**
     * @returns {HTMLInputElement} Search input html element
     * */
    get searchInput() {
        return document.querySelector("#search-category-contents");
    }

    /**
     * Create a categories content service object
     * @param {string} viewType View type to show categories contents
     * @param {string} categoryContentsContainerSelector Selector for categories content container
     * @param {CategoriesService} categoriesService Categories service instance to use
     * @param {string} treeContainerSelector Selector for tree container
     * @param {string} sharedTreeContainerSelector Selector for the shared tree container
     */
    constructor(
        viewType,
        categoryContentsContainerSelector,
        categoriesService,
        treeContainerSelector,
        sharedTreeContainerSelector) {

        this.chartService = new ChartService();
        this.viewType = viewType;
        this.categoryContentsContainerSelector = categoryContentsContainerSelector;
        this.categoriesService = categoriesService;
        this.treeContainerSelector = treeContainerSelector;
        this.sharedTreeContainerSelector = sharedTreeContainerSelector;
        this.openedCharts = [];
        this.editDescriptionModalSelector = "#edit-description-modal";
        this.descriptionTextId = "description-text";
        this.saveDescriptionModalButtonId = "save-description-modal";
        this.sessionStorageCopyKey = "copiedCategoryId";
        this.sessionStorageCopyType = "copyType";
        this.createChartModalSelector = "#create-chart-object-modal";
        this.syncChartsSelector = "#open-sync-charts";
        this.uploadChartDataModalSelector = "#upload-chart-data-modal";
        this.uploadChartDataFormSelector = "#upload-chart-data-form";
        this.chartWindowHeight = 450;
        this.chartWindowWidth = 600;
        this.chartWidth = 500;


        //Register window unload
        this.onWindowUnload();
    }

    /**
     * Searches both regular and shared categories tree and returs the node with the psecified key
     * @param {number} key Key to search for
     * @returns {Fancytree.FancytreeNode} Returns tree node with the given key
     */
    getNodeByKey(key) {
        return this.tree.getNodeByKey('' + key) || this.sharedTree.getNodeByKey('' + key);
    }

    intiCategoriesContent() {

        this.initViewTypeSelector();
        this.registerCategoryContentsClickEvent();
        this.initSearch();
        this.initSyncCharts();

    }

    initViewTypeSelector() {
        this.changeViewTypeButtonStyle();
        $("#viewTypeSelector button").click((event) => {
            this.viewType = event.currentTarget.dataset.viewType;
            this.loadCategoryContent(this.activeNode.key);
            this.changeViewTypeButtonStyle();

        });
    }

    changeViewTypeButtonStyle() {

        switch (this.viewType.toLocaleLowerCase()) {
            case 'list':
                $("#list-view").removeClass("btn-outline-secondary").addClass("btn-outline-primary");
                $("#grid-view").removeClass("btn-outline-primary").addClass("btn-outline-secondary");
                break;
            case 'grid':
                $("#grid-view").removeClass("btn-outline-secondary").addClass("btn-outline-primary");
                $("#list-view").removeClass("btn-outline-primary").addClass("btn-outline-secondary");
                break;
            default:
                $("#list-view").removeClass("btn-outline-secondary").addClass("btn-outline-primary");
                $("#grid-view").removeClass("btn-outline-primary").addClass("btn-outline-secondary");
        }
    }

    /**
     * The only function called to load category contents into the right view. It is called from any component, 
     * in the tree and in the contents view, that wants to load category contents.
     * @param {number} categoryId Id of the category for which to load the contents
     * @param {boolean} fromPopState If this is a call from the browser history pop state
     * @returns {void}
     */
    loadCategoryContent(categoryId, fromPopState = false) {
        if (!categoryId)
            if (this.activeNode && this.activeNode.key)
                categoryId = this.activeNode.key;
            else
                return false;

        this.addLoadingAnimation();
        this.categoriesService.sendGetCategoryContentsRequest(categoryId, this.viewType, fromPopState)
            .done((data) => {
                $(this.categoryContentsContainerSelector).html(data);

                this.initCategoriesContentFunctions();
            }).fail(() => {
                this.removeLoadingAnimation();
            });
    }

    /**
     * Add loading animation to category content container
     **/
    addLoadingAnimation() {
        $(this.categoryContentsContainerSelector).empty().loading('show');
    }

    removeLoadingAnimation() {
        $(this.categoryContentsContainerSelector).find(".spinner").remove();
    }

    initSearch() {
        this.searchInput.addEventListener("blur", (event) => {
            let searchTerm = event.target.value;
            if (!searchTerm) return;

            this.categoriesService.sendSearchCategoriesRequest(searchTerm, this.viewType)
                .done((data) => {
                    $(this.categoryContentsContainerSelector).html(data);
                    $("#breadcrumb").empty();
                    this.initCategoriesContentFunctions();
                });

        });

        this.searchInput.addEventListener("keypress", (event) => {
            let keyCode = event.keyCode ? event.keyCode : event.which;
            if (keyCode === 13)
                event.currentTarget.blur();
        });
    }

    addCategoriesContentContextMenu() {
        $.contextMenu({
            selector: this.categoryContentsContainerSelector + " .category-card,"
                + this.categoryContentsContainerSelector + " .chart-object-card,"
                + this.categoryContentsContainerSelector + " tbody tr,"
                + "#categoriesContentColumn>.card-body",
            items: {
                "create-category": {
                    name: "Create category",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon add-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getTree("#categoriesTree").getActiveNode() || $.ui.fancytree.getTree("#sharedCategoryTree").getActiveNode();
                        if (!node) return false;
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create)
                            && $(this).data("is-category") === undefined;
                    },
                    callback: (itemKey, opt) => {
                        let node = this.activeNode;
                        if (!node) return false;
                        node.setExpanded(true).done(() => {
                            node.editCreateNode("child", {
                                folder: true,
                                title: "",
                                key: null,
                                icon: "folder-icon"
                            });
                        });
                    }
                },
                "create-chart": {
                    name: "Create item",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon add-chart-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getTree("#categoriesTree").getActiveNode() || $.ui.fancytree.getTree("#sharedCategoryTree").getActiveNode();
                        if (!node) return false;
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create)
                            && $(this).data("is-category") === undefined;
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        if (!node) return false;
                        $(this.createChartModalSelector).find("#category-id-input").val(node.key);
                        $(this.createChartModalSelector).modal('show');
                    }
                },
                //"upload-file": {
                //    name: "Upload file",
                //    icon: (opt, $itemElement, itemKey, item) => {
                //        $itemElement.html("<span class='context-menu-icon add-chart-icon'></span> " + item.name);
                //    },
                //    visible: function (key, opt) {
                //        let node = $.ui.fancytree.getTree("#categoriesTree").getActiveNode() || $.ui.fancytree.getTree("#sharedCategoryTree").getActiveNode();
                //        if (!node) return false;
                //            return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create)
                //                && $(this).data("is-category") === undefined;
                //    },
                //    callback: (itemKey, opt) => {
                //        this.uploadChartDataTreeContextCallback(itemKey, opt);
                //    }
                //},
                "editName": {
                    name: "Edit name",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon rename-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        if ($(this).data("is-category") === true) {
                            let categoryData = $(this).data('category');
                            return categoryData.AllowedActions.some(a => a === KGT.Constants.permissions.edit);
                        } else if ($(this).data("is-category") === false) {
                            let chartObjectData = $(this).data('chart-object');
                            return chartObjectData.AllowedActions.some(a => a === KGT.Constants.permissions.edit);
                        }

                        return false;
                    },
                    callback: (itemKey, opt) => {
                        this.editNameContextCallback(itemKey, opt);
                    }
                },
                "editDescription": {
                    name: "Edit description",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon rename-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        if ($(this).data("is-category")) {
                            let categoryData = $(this).data('category');
                            return categoryData.AllowedActions.some(a => a === KGT.Constants.permissions.edit);
                        } else if ($(this).data("is-category") === false) {
                            let chartObjectData = $(this).data('chart-object');
                            return chartObjectData.AllowedActions.some(a => a === KGT.Constants.permissions.edit);
                        }

                        return false;
                    },
                    callback: (itemKey, opt) => {
                        this.editDescriptionContextCallback(itemKey, opt);
                    }
                },
                "copy": {
                    name: "Copy",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon copy-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        if ($(this).data("is-category") === true) {
                            let categoryData = $(this).data('category');
                            return categoryData.AllowedActions.some(a => a === KGT.Constants.permissions.copy);
                        } else if ($(this).data("is-category") === false) {
                            let chartObjectData = $(this).data("chart-object");
                            return chartObjectData.AllowedActions.some(a => a === KGT.Constants.permissions.copy);
                        }

                        return false;
                    },
                    callback: (itemKey, opt) => {
                        if (opt.$trigger.data("is-category")) {
                            let categoryData = opt.$trigger.data('category');
                            sessionStorage.setItem(this.sessionStorageCopyKey, categoryData.Id);
                            sessionStorage.setItem(this.sessionStorageCopyType, "category");
                        } else {
                            let chartObjecgtData = opt.$trigger.data('chart-object');
                            sessionStorage.setItem(this.sessionStorageCopyKey, chartObjecgtData.Id);
                            sessionStorage.setItem(this.sessionStorageCopyType, "chartObject");
                        }
                    }
                },
                "paste": {
                    name: "Paste",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon copy-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        if ($(this).data("is-category")) {
                            let categoryData = $(this).data('category');
                            return categoryData.AllowedActions.some(a => a === KGT.Constants.permissions.create) && sessionStorage.getItem("copiedCategoryId");
                        }

                        //Return false if it is chart object
                        return false;
                    },
                    callback: (itemKey, opt) => {
                        if (opt.$trigger.data("is-category")) {
                            let categoryData = opt.$trigger.data('category');
                            if (sessionStorage.getItem(this.sessionStorageCopyType) === "category") {
                                this.copyCategory(sessionStorage.getItem(this.sessionStorageCopyKey), categoryData.Id);
                            } else {
                                this.copyChartObject(sessionStorage.getItem(this.sessionStorageCopyKey), categoryData.Id);
                            }
                        }
                    }
                },
                "delete": {
                    name: "Delete",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon delete-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        if ($(this).data("is-category")) {
                            let categoryData = $(this).data('category');
                            return categoryData.AllowedActions.some(a => a === KGT.Constants.permissions.delete);
                        } else if ($(this).data("is-category") === false) {
                            let chartObjecgtData = $(this).data('chart-object');
                            return chartObjecgtData.AllowedActions.some(a => a === KGT.Constants.permissions.edit);
                        }

                        return false;
                    },
                    callback: (itemKey, opt) => {
                        this.deleteContextCallback(itemKey, opt);
                    }
                }
            }
        });
    }

    editNameContextCallback(itemKey, opt) {
        let element = opt.$trigger;
        let isCategory = element.data('is-category');
        let orgText = isCategory ? element.data('category').Title : element.data('chart-object').Title;
        let input = $("<input />", {
            value: orgText,
            type: 'text',
            click: (e) => {
                e.stopImmediatePropagation();
            },
            blur: (event) => {
                let newText = $(event.currentTarget).val();
                if (!newText || newText === orgText) {
                    $(event.currentTarget).replaceWith(orgText);
                } else if (isCategory) {
                    let categoryId = element.data('category').Id;
                    this.categoriesService.sendRenameCategoryRequest(categoryId, newText)
                        .done((data) => {
                            input.replaceWith(data.title);
                            this.getNodeByKey('' + categoryId).setTitle(newText);
                            this.loadCategoryContent(this.activeNode.key);
                        });
                } else {
                    let chartObjectId = element.data('chart-object').Id;
                    this.categoriesService.sendRenameChartObjectRequest(chartObjectId, newText)
                        .done((data) => {
                            input.replaceWith(data.title);
                            this.loadCategoryContent(this.activeNode.key);
                        });
                }
            },
            keypress: (event) => {
                let keyCode = event.keyCode ? event.keyCode : event.which;
                if (keyCode === 13)
                    input.blur();
            }
        });

        if (this.viewType.toLocaleLowerCase() === 'grid') {
            element.find('.card-footer small').empty().append(input);
        } else if (this.viewType.toLocaleLowerCase() === 'list') {
            element.find('.title-cell').empty().append(input);
        }

        input.focus();
    }

    editDescriptionContextCallback(itemKey, opt) {
        let element = opt.$trigger;
        let isCategory = element.data('is-category');
        let elData = isCategory ? element.data('category') : element.data('chart-object');
        let orgText = elData.Description;


        document.getElementById(this.descriptionTextId).value = orgText;
        document.getElementById(this.saveDescriptionModalButtonId).onclick = (event) => {
            let newDescription = document.getElementById(this.descriptionTextId).value;
            if (newDescription === orgText) return;

            let request = null;
            if (isCategory) {
                request = this.categoriesService.sendEditCategoryDescriptionRequest(elData.Id, newDescription);
            } else {
                request = this.categoriesService.sendEditChartObjectDescriptionRequest(elData.Id, newDescription);
            }

            request.done(() => {
                this.loadCategoryContent(this.activeNode.key);
                $(this.editDescriptionModalSelector).modal("hide");
            });
        };

        $(this.editDescriptionModalSelector).modal("show");
    }

    copyCategory(categoryToCopyId, parentCategoryId) {
        this.categoriesService.sendCopyCategoryRequest(categoryToCopyId, parentCategoryId)
            .done(() => {
                let parentCategory = this.getNodeByKey('' + parentCategoryId);
                if (parentCategory) {

                    let isExpanded = parentCategory.isExpanded();
                    parentCategory.resetLazy();

                    if (isExpanded)
                        parentCategory.setExpanded();

                    if (parentCategory.isActive())
                        this.dispatchLoadCategoryContentsEvent(parentCategory.key);
                }
            });
    }

    dispatchLoadCategoryContentsEvent(categoryId) {
        const loadCategoryEvent = new CustomEvent('loadCategoryContents', {
            bubbles: true,
            detail: {
                categoryId: categoryId
            }
        });

        document.querySelector(this.treeContainerSelector).dispatchEvent(loadCategoryEvent);
    }

    copyChartObject(chartObjectToCopyId, parentCategoryId) {
        this.categoriesService.sendCopyChartObjectRequest(chartObjectToCopyId, parentCategoryId)
            .done(() => {
                let parentCategory = this.getNodeByKey('' + parentCategoryId);
                if (parentCategory && parentCategory.isActive()) {

                    this.loadCategoryContent(parentCategory.key);
                }
            });
    }

    deleteContextCallback(itemKey, opt) {
        if (opt.$trigger.data('is-category')) {
            this.categoriesService.sendDeleteCategoryRequest(opt.$trigger.data('category').Id)
                .done(() => {
                    let removedNode = this.getNodeByKey('' + opt.$trigger.data('category').Id);
                    if (removedNode)
                        removedNode.remove();
                    this.loadCategoryContent(this.activeNode.key);
                });
        } else {
            this.categoriesService.sendDeleteChartObjectRequest(opt.$trigger.data('chart-object').Id)
                .done(() => {
                    this.loadCategoryContent(this.activeNode.key);
                });
        }
    }

    addContentViewContextMenu() {
        $.contextMenu({
            selector: "#categoriesContentColumn .card-body>:not(.row)",
            items: {
                "create-category": {
                    name: "Create category",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon add-folder-icon'></span> " + item.name);
                    },
                    callback: (itemKey, opt) => {
                        let node = this.activeNode;
                        if (!node) return false;
                        node.setExpanded(true).done(() => {
                            node.editCreateNode("child", {
                                folder: true,
                                title: "",
                                key: null
                            });
                        });
                    }
                }
            }
        });
    }

    initCategoriesContentFunctions() {
        switch (this.viewType.toLocaleLowerCase()) {
            case "grid":
                this.initDraggableCards();
                break;
            case "list":
                this.initDataTables();
                this.initDraggableTableRows();
                break;
            default:
                this.initDraggableCards();
                this.addContentViewContextMenu();
        }

        this.initDroppableElements();

        //this.addContentViewContextMenu();
        this.addCategoriesContentContextMenu();

        this.registerChartObjectClickEvent();
    }

    initDataTables() {
        this.table = $(this.categoryContentsContainerSelector).find("table").DataTable({
            paging: false,
            searching: false,
            info: false,
            order: [],
            columnDefs: [{
                targets: [0],
                searchable: false,
                orderable: false,
                width: "40px"
            }],
            autoWidth: true,
            dom: 'Rlfrtip'
        });
    }

    initDraggableCards() {
        $(this.categoryContentsContainerSelector).find(".category-card,.chart-object-card").draggable({
            revert: "invalid", //"invalid",
            cursorAt: { top: -5, left: -5 },
            connectToFancytree: true,   // let Fancytree accept drag events
            helper: "clone",
            appendTo: "body",
            start: (event, ui) => {
                let objectData = $(event.target).data('is-category') ? $(event.target).data('category') : $(event.target).data('chart-object');

                if (!$(event.target).data('is-category') && objectData.IsShared && !IS_ADMIN)
                    return false;

                return !objectData.IsRoot;
            }
        });
    }

    initDraggableTableRows() {
        $(this.categoryContentsContainerSelector).find("tbody tr").draggable({
            revert: "invalid", //"invalid",
            cursorAt: { top: -5, left: -5 },
            connectToFancytree: true,   // let Fancytree accept drag events
            helper: function (event) {
                let target = $(event.currentTarget);
                let originalParent = target.parents("table").first();

                let tableBody = $("<tbody/>")
                    .append($(event.currentTarget).clone());
                let table = $("<table/>", {
                    class: "display compact dataTable no-footer",
                    role: "grid"
                }).append(tableBody);

                table.css("width", event.currentTarget.offsetWidth);
                return table;
            },
            appendTo: "body",
            start: (event, ui) => {
                let objectData = $(event.target).data('is-category') ? $(event.target).data('category') : $(event.target).data('chart-object');

                if (!$(event.target).data('is-category') && objectData.IsShared && !IS_ADMIN)
                    return false;

                return !objectData.IsRoot;
            }
        });
    }

    initDroppableElements() {
        $(this.categoryContentsContainerSelector).find(".category-card,.category-row").droppable({
            accept: ".category-card,.chart-object-card,.category-row,.chart-object-row",
            tolerance: "pointer",
            drop: (event, ui) => {
                if ($(event.target).data("category").AllowedActions
                    && $(event.target).data("category").AllowedActions.some(a => a === KGT.Constants.permissions.create)) {
                    this.onElementDrop(event, ui);
                }
                else {
                    return false;
                }
            }
        });
    }

    onElementDrop(event, ui) {
        let $draggedElement = ui.draggable;
        let $targetElement = $(event.target);

        if ($draggedElement.data('is-category')) {
            this.onCategoryDropped($draggedElement, $targetElement);
        } else {
            this.onChartObjectDropped($draggedElement, $targetElement);
        }
    }

    onCategoryDropped($category, $target) {
        let nodeToMove = this.getNodeByKey('' + $category.data('category').Id);
        let targetNode = this.getNodeByKey('' + $target.data('category').Id);
        this.moveCategory(nodeToMove, targetNode);
    }

    moveCategory(nodeToMove, targetNode) {

        this.categoriesService.sendMoveCategoryRequest(nodeToMove.key, targetNode.key)
            .done(() => {
                if (!nodeToMove || !targetNode) {
                    if (nodeToMove) {
                        nodeToMove.remove();
                    }

                    if (targetNode) {
                        targetNode.resetLazy();
                        targetNode.load(true);
                    }
                }

                let parentNodeToMove = nodeToMove.parent;
                if (targetNode.isLazy()) {
                    targetNode.load().done(() => {
                        nodeToMove.moveTo(targetNode, "child");
                    });
                } else {
                    nodeToMove.moveTo(targetNode, "child");
                }

                if (targetNode.isActive())
                    this.loadCategoryContent(this.activeNode.key);

                if (parentNodeToMove.isActive()) {
                    this.loadCategoryContent(parentNodeToMove.key);
                }
            });


    }

    onChartObjectDropped($chartObject, $target) {
        let chartObjectId = $chartObject.data('chart-object').Id;
        let newCategoryCardId = $target.data('category').Id;
        if (chartObjectId && newCategoryCardId)
            this.categoriesService.sendChangeChartObjectCategoryRequest(chartObjectId, newCategoryCardId)
                .done(() => {
                    if (this.activeNode)
                        this.loadCategoryContent(this.activeNode.key);
                });
    }

    registerCategoryContentsClickEvent() {
        $(this.categoryContentsContainerSelector).on("click", ".category-card,.category-row", (event) => {
            const categoryData = $(event.currentTarget).data('category');
            this.openCategory(categoryData.Id);
        });
    }

    registerChartObjectClickEvent() {
        $(".chart-object-row,.chart-object-card").click("click", (event) => {
            const $chartEl = $(event.delegateTarget);
            const chartData = $chartEl.data("chart-object");

            this.openChart(chartData);
        });
    }

    openChart(chartData) {
        //Check if chart is already opend
        let openedChart = this.openedCharts.find(oc => oc.id === chartData.Id);
        if (openedChart) {
            openedChart.window.focus();
            return;
        }

        $("#" + this.getChartElementId(chartData.Id)).addClass("chart-selected");

        let chartWindow = ChartWindowManager.setUpChartWindow(chartData, this.openedCharts);

        this.chartService.sendGetChartOptionsRequest(chartData.Id).done((data) => {
            let syncChartService = SyncCharts.createCharts(chartWindow.document.body, [data], ChartWindowManager.chartWidth);
            openedChart = {
                id: chartData.Id,
                data: data,
                window: chartWindow,
                chart: syncChartService.getCharts()[0],
                chartService: syncChartService,
                isSynced: false
            };
            
            this.openedCharts.push(openedChart);
        }).fail(() => {
            chartWindow.close();
        });
    }

    openCategory(categoryId) {
        let categoryNodeToOpen = this.getNodeByKey('' + categoryId);
        if (categoryNodeToOpen)
            categoryNodeToOpen.setActive();
        else {
            this.loadCategoryContent(categoryId);
        }
    }

    clearSearch() {
        this.searchInput.value = "";
    }

    getChartElementId(id) {
        return "chart-" + id;
    }

    onWindowUnload() {
        window.onunload = () => {
            this.openedCharts.forEach(oc => {
                oc.window.close();
            });
        };
    }

    initSyncCharts() {
        $(this.syncChartsSelector).click(() => {
            this.syncOpenedCharts();
        });
    }

    syncOpenedCharts() {
        let syncChartService = null;

        if (this.openedCharts && this.openedCharts.length < 2) {
            alert("Open two or more charts to synchronize");
            return;
        }
        if (this.openedCharts && this.openedCharts.length > 1) {
            this.openedCharts.forEach((openedChart, index) => {
                openedChart.chart.destroy();
                openedChart.window.document.body.innerHTML = "";
                openedChart.isSynced = true;
                openedChart.data.data.firstRowAsNames = false;

                if (syncChartService) {
                    let chart = syncChartService.addChart(openedChart.data, openedChart.window.document.body);
                    openedChart.chart = chart;
                    openedChart.chartService = syncChartService;
                } else {
                    syncChartService = SyncCharts.createCharts(openedChart.window.document.body, [openedChart.data], this.chartWidth);
                    openedChart.chart = syncChartService.getCharts()[0];
                    openedChart.chartService = syncChartService;
                }
                $(openedChart.window.document.body).prepend($("<span />", {
                    text: "Synced",
                    class: "synced-notification"
                }));

                openedChart.window.resizeTo(ChartWindowManager.chartWindowWidth, ChartWindowManager.chartWindowHeight + 50);
            });
        }
    }

}