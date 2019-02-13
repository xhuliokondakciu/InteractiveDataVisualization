class TreeManager {

    get activeNode() {
        return this.tree.activeNode || this.sharedTree.activeNode;
    }

    constructor(treeContainerSelector, sharedTreeContainerSelector, categoriesService, hierarchyToOpen) {
        this.treeContainerSelector = treeContainerSelector;
        this.categoriesService = categoriesService;
        this.hierarchyToOpen = hierarchyToOpen;
        this.hierarchyToOpenShared = hierarchyToOpen.slice(0, hierarchyToOpen.length);
        this.sharedTreeContainerSelector = sharedTreeContainerSelector;

        //Edit description modal selectors
        this.editDescriptionModalSelector = "#edit-description-modal";
        this.descriptionTextId = "description-text";
        this.saveDescriptionModalButtonId = "save-description-modal";
        this.sessionStorageCopyKey = "copiedCategoryId";
        this.sessionStorageCopyType = "copyType";
        this.createChartModalSelector = "#create-chart-object-modal";
        this.uploadChartDataModalSelector = "#upload-chart-data-modal";
        this.uploadChartDataFormSelector = "#upload-chart-data-form";

        $.connection.chartHub.client.reloadNode = (categoryId) => {
            if (this.activeNode.key == categoryId)
                this.dispatchNodeActivatedEvent(categoryId);

            let categoryNode = this.getNodeByKey(categoryId);
            if (categoryNode) {
                categoryNode.load(true);
            }
        };
    }

    initTree() {

        this.tree = this.constructTree();
        this.sharedTree = this.constructSharedTree();
        this.addTreeContextMenu();
        this.addSharedTreeContextMenu();
        this.registerCopyPasteKeyEvents();

        return this.tree;
    }

    /**
     * Get node by key either in the user tree or in the shared categories tree
     * @param {string} key The key of the node to get
     * @returns {Fancytree.FancytreeNode} The found node
     */
    getNodeByKey(key) {
        return this.tree.getNodeByKey('' + key) || this.sharedTree.getNodeByKey('' + key);
    }

    /**
     * Construct user categories tree
     * @returns {void}
     */
    constructTree() {
        this.treeOptions = this.constructTreeOptions();

        if (this.treeOptions)
            return $.ui.fancytree.createTree(this.treeContainerSelector, this.treeOptions);
    }

    /**
     * Consruct the tree options object to use for the user categores tree
     * @returns {void}
     */
    constructTreeOptions() {

        return {
            extensions: ["dnd", "edit", "persist"],
            selectMode: 1,
            autoScroll: true,
            clickFolderMode: 4,
            keyboard: true,
            quicksearch: true,
            tooltip: true,
            source: {
                //ToDo: change to injected workspaceAPI
                url: workspaceApi.treeSourceUrl
            },
            lazyLoad: (event, data) => {
                this.lazyLoad(event, data);
            },
            activate: (event, data) => {
                this.onNodeActivated(event, data);

                //So we have just one active node at a time in both trees
                if (this.sharedTree.activeNode)
                    this.sharedTree.activeNode.setActive(false);
            },
            //persist: {
            //    expandLazy: true,
            //    store: "auto"
            //},
            restore: () => {
                if (this.hierarchyToOpen)
                    this.openTreeHierarchy(this.hierarchyToOpen);
            },
            dnd: {
                autoExpandMS: 400,
                focusOnClick: true,
                preventVoidMoves: true, // Prevent dropping nodes 'before self', etc.
                preventRecursiveMoves: true, // Prevent dropping nodes on own descendants
                smartRevert: true,
                draggable: {
                    containment: $(this.treeContainerSelector).parent('.card')
                },
                dragStart: (node, data) => {
                    /** This function MUST be defined to enable dragging for the tree.
                     *  Return false to cancel dragging of node.
                     */


                    return !node.data.isRoot;
                },
                dragEnter: (node, data) => {
                    return this.onTreeDragEnter(node, data);
                },
                dragDrop: (node, data) => {
                    return this.onTreeDragDrop(node, data);
                }
            },
            edit: {
                triggerStart: ["f2", "mac+enter", "shift+click"],
                save: (event, data) => {
                    this.editNode(event, data);
                },
                beforeEdit: (event, data) => {
                    if (data.isNew) {
                        return true;
                    }
                    return data.node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
                }
            }
        };
    }

    constructSharedTree() {
        this.sharedTreeOptions = this.constructSharedTreeOptions();

        if (this.sharedTreeOptions)
            return $.ui.fancytree.createTree(this.sharedTreeContainerSelector, this.sharedTreeOptions);
    }

    constructSharedTreeOptions() {
        return {
            extensions: ["dnd", "edit", "persist"],
            selectMode: 1,
            autoScroll: true,
            clickFolderMode: 4,
            keyboard: true,
            quicksearch: true,
            tooltip: true,
            source: {
                //ToDo: change to injected workspaceAPI
                url: workspaceApi.getSharedCategories
            },
            lazyLoad: (event, data) => {
                data.result = {
                    url: `${workspaceApi.getSharedCategories}?parentId=${data.node.key}`
                };
            },
            activate: (event, data) => {
                this.onNodeActivated(event, data);
                if (this.tree.activeNode)
                    this.tree.activeNode.setActive(false);
            },
            restore: () => {
                if (this.hierarchyToOpenShared)
                    this.openTreeHierarchy(this.hierarchyToOpenShared);
            },
            dnd: {
                autoExpandMS: 400,
                focusOnClick: true,
                preventVoidMoves: true, // Prevent dropping nodes 'before self', etc.
                preventRecursiveMoves: true, // Prevent dropping nodes on own descendants
                smartRevert: true,
                draggable: {
                    containment: $(this.treeContainerSelector).parent('.card')
                },
                dragStart: (node, data) => {
                    /** This function MUST be defined to enable dragging for the tree.
                     *  Return false to cancel dragging of node.
                     */

                    return !node.data.isRoot;
                },
                dragEnter: (node, data) => {

                    if (IS_ADMIN) {
                        if (node.data.isSharedRoot) {
                            return false;
                        }

                        return true;
                    } else if (!node.data.isEveryones) {
                        return false;
                    }

                    return this.onTreeDragEnter(node, data);
                },
                dragDrop: (node, data) => {
                    if (!node.data.isEveryones && !IS_ADMIN)
                        return false;

                    return this.onTreeDragDrop(node, data);
                }
            },
            edit: {
                triggerStart: ["f2", "mac+enter", "shift+click"],
                save: (event, data) => {
                    this.editNode(event, data);
                },
                beforeEdit: (event, data) => {
                    if (data.isNew)
                        return true;
                    return data.node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
                }

            }
        };
    }

    /**
     * Callback to call to edit node
     * @param {any} event Event object
     * @param {any} data Event data
     * @returns {void}
     */
    editNode(event, data) {
        let newTitle = data.input.val();
        if (data.isNew) {
            this.categoriesService.sendCreateCategoryRequest(data.node.parent.key, newTitle)
                .done((categoryCreated) => {
                    if (categoryCreated && categoryCreated.key) {
                        let parentNode = data.node.parent;

                        parentNode.addNode(categoryCreated);

                        data.node.remove();

                        if (parentNode.isActive()) {
                            this.dispatchLoadCategoryContentsEvent(parentNode.key);
                        }
                    } else {
                        console.error("Couldn't create category with name: " + newTitle);
                        data.node.remove();
                    }
                }).fail((error) => {
                    console.error("Couldn't create category with name: " + newTitle, error);
                    data.node.remove();
                });
        } else if (data.dirty) {
            this.categoriesService.sendRenameCategoryRequest(data.node.key, newTitle)
                .done((category) => {
                    data.node.setTitle(category.title);
                    if (data.node.parent.isActive()) {
                        this.dispatchLoadCategoryContentsEvent(data.node.parent.key);
                    }
                }).fail((error) => {
                    console.error("Couldn't rename category with name: " + newTitle, error);
                    data.node.remove();
                });
        }
    }

    /**
    * Sends copy category requests and handles the tree state after a successful copy
    * @param {any} categoryToCopyId Category to copy id
    * @param {any} parentCategoryId Parent category id
    * @returns {void}
    */
    copyCategory(categoryToCopyId, parentCategoryId) {
        this.categoriesService.sendCopyCategoryRequest(categoryToCopyId, parentCategoryId)
            .done(() => {
                let parentCategory = this.getNodeByKey(parentCategoryId);
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

    /**
     * Callback to call for tree lazy load
     * @param {any} event Event of callback
     * @param {Fancytree.EventData} data Data of event
     * @returns {void}
     */
    lazyLoad(event, data) {
        data.result = {
            url: `${workspaceApi.treeSourceUrl}?parentId=${data.node.key}`
        };
    }

    /**
     * Callback to call when node is activated
     * @param {any} event Callback event
     * @param {Fancytree.EventData} data Event data
     * @returns {void}
     */
    onNodeActivated(event, data) {
        if (data.node.lazy)
            data.node.load();

        this.constructBreadcrumb(data.node);

        this.dispatchNodeActivatedEvent(data.node.key);
    }

    /**
     * Callback for on draggable drop on tree node
     * @param {Fancytree.FancytreeNode} node Node where element was dropped 
     * @param {any} data Data
     * @returns {void}
     */
    onTreeDragDrop(node, data) {
        let otherNode;
        let draggedElementId;
        let targetElementId = node.key;
        let draggedIsSharedCategory;
        let targetIsEveryones = node.data.isEveryones;
        let draggedIsEveryones;

        if (!data.otherNode) {
            // It's a non-tree draggable
            if (data.draggable.element.data("is-category")) {
                let categoryData = data.draggable.element.data("category");
                if (!categoryData) return false;

                draggedElementId = categoryData.Id;
                draggedIsSharedCategory = categoryData.IsShared;
                draggedIsEveryones = categoryData.IsEveryones;
            } else {
                draggedElementId = data.draggable.element.data('chart-object').Id;
                this.categoriesService.sendChangeChartObjectCategoryRequest(draggedElementId, targetElementId)
                    .done(() => {
                        this.dispatchLoadCategoryContentsEvent(this.activeNode.key);
                    });
                return;
            }
        } else {
            draggedElementId = data.otherNode.key;
            draggedIsSharedCategory = data.otherNode.data.isShared;
            draggedIsEveryones = data.otherNode.data.isEveryones;
        }

        //If is admin move is always allowed so always move
        //If dragged node is shared copy
        //If target node is everyones and dragged node is not everyones copy
        //If target node is everyones and dragged node is everyones move
        if (draggedIsEveryones && targetIsEveryones) {
            this.moveCategory(draggedElementId, targetElementId);
        } else if (IS_ADMIN) {
            this.moveCategory(draggedElementId, targetElementId);
        } else if (draggedIsEveryones && !targetIsEveryones) {
            this.copyCategory(draggedElementId, targetElementId);
        } else if (draggedIsSharedCategory || targetIsEveryones && !draggedIsEveryones) {
            this.copyCategory(draggedElementId, targetElementId);
        } else {
            this.moveCategory(draggedElementId, targetElementId);
        }

    }

    onTreeDragEnter(node, data) {
        /** data.otherNode may be null for non-fancytree droppables.
                    *  Return false to disallow dropping on node. In this case
                    *  dragOver and dragLeave are not called.
                    *  Return 'over', 'before, or 'after' to force a hitMode.
                    *  Return ['before', 'after'] to restrict available hitModes.
                    *  Any other return value will calc the hitMode from the cursor position.
                    */
        // Prevent dropping a parent below another parent (only sort
        // nodes under the same parent)
        /*           if(node.parent !== data.otherNode.parent){
                    return false;
                  }
                  // Don't allow dropping *over* a node (would create a child)
                  return ["before", "after"];
        */

        if (!data.otherNode) {
            // It's a non-tree draggable
            if (data.draggable.element.data("is-category")) {
                let categoryData = data.draggable.element.data("category");

                //node.key is string so we have to convert it to an int
                if (+node.key === categoryData.Id) return false;

                // Block dragging if we are inside subcategories of the dragged categorie
                let parentNode = node.parent;
                while (parentNode) {
                    if (+parentNode.key === categoryData.Id)
                        return false;

                    parentNode = parentNode.parent;
                }
            }
        } else {
            //Can't drop on node own parent
            if (data.otherNode.parent && node.key === data.otherNode.parent.key)
                return false;
        }

        return ["over"];
    }

    addTreeContextMenu() {
        $.contextMenu({
            selector: this.treeContainerSelector + " span.fancytree-title",
            items: {
                "create-category": {
                    name: "Create category",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon add-folder-icon'></span> " + item.name);
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
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
                    callback: (itemKey, opt) => {
                        this.createChartTreeContextCallback(itemKey, opt);
                    }
                },
                //"upload-file": {
                //    name: "Upload file",
                //    icon: (opt, $itemElement, itemKey, item) => {
                //        $itemElement.html("<span class='context-menu-icon add-chart-icon'></span> " + item.name);
                //    },
                //    visible: function (key, opt) {
                //        let node = $.ui.fancytree.getNode(this);
                //        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create);
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
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
                    },
                    callback: (itemKey, opt) => {
                        return this.startEditNode(itemKey, opt);
                    }
                },
                "editDescription": {
                    name: "Edit description",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon rename-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
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
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.copy);
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        sessionStorage.setItem(this.sessionStorageCopyKey, node.key);
                        sessionStorage.setItem(this.sessionStorageCopyType, 'category');
                    }
                },
                "paste": {
                    name: "Paste",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon paste-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create) && sessionStorage.getItem("copiedCategoryId");
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        let objectToCopyId = sessionStorage.getItem(this.sessionStorageCopyKey);

                        if (sessionStorage.getItem(this.sessionStorageCopyType) === "category") {
                            this.copyCategory(objectToCopyId, node.key);
                        } else {
                            this.copyChartObject(objectToCopyId, node.key);
                        }
                    }
                },
                "delete": {
                    name: "Delete",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon delete-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.delete);
                    },
                    callback: (itemKey, opt) => {
                        return this.deleteNode(itemKey, opt);
                    }
                }
            }
        });
    }

    addSharedTreeContextMenu() {
        $.contextMenu({
            selector: this.sharedTreeContainerSelector + " span.fancytree-title",
            items: {
                "create-category": {
                    name: "Create category",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon add-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create);
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        node.setExpanded(true).done(() => {
                            node.editCreateNode("child", {
                                folder: true,
                                title: "",
                                key: null,
                                icon: "folder-icon",
                                width: "100%"
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
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create);
                    },
                    callback: (itemKey, opt) => {
                        this.createChartTreeContextCallback(itemKey, opt);
                    }
                },
                "editName": {
                    name: "Edit name",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon rename-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
                    },
                    callback: (itemKey, opt) => {
                        return this.startEditNode(itemKey, opt);
                    }
                },
                "editDescription": {
                    name: "Edit description",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon rename-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.edit);
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
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.copy);
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        sessionStorage.setItem(this.sessionStorageCopyKey, node.key);
                        sessionStorage.setItem(this.sessionStorageCopyType, 'category');
                    }
                },
                "paste": {
                    name: "Paste",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon paste-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.create) && sessionStorage.getItem("copiedCategoryId");
                    },
                    callback: (itemKey, opt) => {
                        let node = $.ui.fancytree.getNode(opt.$trigger);
                        let objectToCopyId = sessionStorage.getItem(this.sessionStorageCopyKey);
                        var nodeWhereToCopy = this.getNodeByKey(objectToCopyId);

                        if (sessionStorage.getItem(this.sessionStorageCopyType) === "category") {
                            this.copyCategory(objectToCopyId, node.key);
                        } else {
                            this.copyChartObject(objectToCopyId, node.key);
                        }
                    }
                },
                "delete": {
                    name: "Delete",
                    icon: (opt, $itemElement, itemKey, item) => {
                        $itemElement.html("<span class='context-menu-icon delete-folder-icon'></span> " + item.name);
                    },
                    visible: function (key, opt) {
                        let node = $.ui.fancytree.getNode(this);
                        return node.data.allowedActions.some(a => a === KGT.Constants.permissions.delete);
                    },
                    callback: (itemKey, opt) => {
                        return this.deleteNode(itemKey, opt);
                    }
                }

            }
        });
    }

    createChartTreeContextCallback(itemKey, opt) {
        let node = $.ui.fancytree.getNode(opt.$trigger);
        if (!node) return false;
        $(this.createChartModalSelector).find("form").trigger("reset");
        $(this.createChartModalSelector).find("#category-id-input").val(node.key);
        $(this.createChartModalSelector).modal('show');
    }

    copyChartObject(chartObjectToCopyId, parentCategoryId) {
        this.categoriesService.sendCopyChartObjectRequest(chartObjectToCopyId, parentCategoryId)
            .done(() => {
                let parentCategory = this.getNodeByKey(parentCategoryId);
                if (parentCategory && parentCategory.isActive()) {
                    this.dispatchLoadCategoryContentsEvent(parentCategory.key);
                }
            });
    }

    startEditNode(itemKey, opt) {
        let node = $.ui.fancytree.getNode(opt.$trigger);
        node.editStart();
    }

    editDescriptionContextCallback(itemKey, opt) {
        let node = $.ui.fancytree.getNode(opt.$trigger);
        let orgText = node.data.description;


        document.getElementById(this.descriptionTextId).value = orgText;
        document.getElementById(this.saveDescriptionModalButtonId).onclick = (event) => {
            let newDescription = document.getElementById(this.descriptionTextId).value;
            if (!newDescription || newDescription === orgText) return;

            let request = null;
            this.categoriesService.sendEditCategoryDescriptionRequest(node.key, newDescription)
                .done(() => {
                    this.dispatchLoadCategoryContentsEvent(this.activeNode.key);
                    $(this.editDescriptionModalSelector).modal("hide");
                });
        };

        $(this.editDescriptionModalSelector).modal("show");
    }

    deleteNode(itemKey, opt) {
        let node = $.ui.fancytree.getNode(opt.$trigger);
        let nodeParent = node.parent;
        this.categoriesService.sendDeleteCategoryRequest(node.key)
            .done(() => {
                if (node.isActive()) {
                    nodeParent.setActive();
                } else if (nodeParent.isActive()) {
                    this.dispatchLoadCategoryContentsEvent(nodeParent.key);
                }
                node.remove();
            });

    }

    moveCategory(nodeToMoveId, targetNodeId) {

        this.categoriesService.sendMoveCategoryRequest(nodeToMoveId, targetNodeId)
            .done((data) => {
                let nodeToMove = this.getNodeByKey(nodeToMoveId);
                let targetNode = this.getNodeByKey(targetNodeId);

                if (nodeToMove)
                    //Title might change if node has same title with another node in the new parent
                    nodeToMove.setTitle(data.title);

                if (!nodeToMove || !targetNode) {
                    if (nodeToMove) {
                        nodeToMove.remove();
                    }

                    if (targetNode) {
                        targetNode.resetLazy();
                        targetNode.load(true);
                    }

                    return;
                }

                let oldParent;
                if (targetNode && targetNode.isLazy()) {
                    targetNode.load().done(() => {
                        nodeToMove.moveTo(targetNode, "child");
                    });
                } else if (nodeToMove) {
                    oldParent = nodeToMove.parent;
                    nodeToMove.moveTo(targetNode, "child");
                }

                if (oldParent && oldParent.isActive())
                    this.dispatchLoadCategoryContentsEvent(oldParent.key);
                else if (targetNode && targetNode.isActive())
                    this.dispatchLoadCategoryContentsEvent(targetNode.key);


            });
    }

    constructBreadcrumb(node) {
        let $breadcrumbContainer = $("#breadcrumb");
        $breadcrumbContainer.empty();
        let parentNodes = node.getParentList();
        for (let parent of parentNodes) {
            let breadCrumb = $("<li />", {
                class: "breadcrumb-item"
            }).append($("<a />", {
                text: parent.title,
                href: "#",
                click: (e) => {
                    e.preventDefault();
                    let node = this.getNodeByKey('' + parent.key);
                    node.setActive();
                }
            }));
            $breadcrumbContainer.append(breadCrumb);
        }

        let activeBreadcrumb = $("<li />", {
            class: "breadcrumb-item active",
            text: node.title
        }).appendTo($breadcrumbContainer);
    }

    setNodeActive(nodeKey, noEvents) {
        let node = this.getNodeByKey(nodeKey);
        if (node) {
            node.setActive(true, {
                noEvents: noEvents
            });
        }
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

    dispatchNodeActivatedEvent(categoryId) {
        const nodeActivatedEvent = new CustomEvent('nodeActivated', {
            bubbles: true,
            detail: {
                categoryId: categoryId
            }
        });
        document.querySelector(this.treeContainerSelector + "," + this.sharedTreeContainerSelector).dispatchEvent(nodeActivatedEvent);
    }

    registerCopyPasteKeyEvents() {
        let ctrlDown = false,
            ctrlKey = 17,
            cmdKey = 91,
            vKey = 86,
            cKey = 67;

        $(document).keydown((e) => {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = true;
        }).keyup(function (e) {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = false;
        });

        $(this.treeContainerSelector).keydown((e) => {
            if (!this.activeNode) return;

            if (ctrlDown && e.keyCode === cKey) {
                e.stopImmediatePropagation();
                if (!this.activeNode.data.isRoot) {
                    sessionStorage.setItem(this.sessionStorageCopyKey, this.activeNode.data.key);
                }
            } else if (ctrlDown && e.keyCode === vKey) {
                e.stopImmediatePropagation();
                if (this.activeNode.data.isEveryones || !this.activeNode.data.isShared) {
                    this.copyCategory(sessionStorage.getItem(this.sessionStorageCopyKey), this.activeNode.key);
                }
            }
        });
    }

    /**
     * Open tree nodes until a specific folder
     * @param {any} hierarchy A list of folders to open with the lowest level at the start and the highest level at the end
     */
    openTreeHierarchy(hierarchy) {
        if (typeof hierarchy !== 'object') return;
        if (hierarchy.length === 1) {
            let node = this.getNodeByKey('' + hierarchy.pop());
            if (node)
                node.setActive();
        } else if (hierarchy.length) {
            let node = this.getNodeByKey('' + hierarchy.pop());

            if (node) {
                node.setExpanded()
                    .done(() => {
                        this.openTreeHierarchy(hierarchy);
                    });
            }

        }
    }
}