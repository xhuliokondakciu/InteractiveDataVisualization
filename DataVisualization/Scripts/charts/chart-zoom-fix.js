(function (H) {

    var addEvent = H.addEvent,
        attr = H.attr,
        charts = H.charts,
        color = H.color,
        css = H.css,
        defined = H.defined,
        extend = H.extend,
        find = H.find,
        fireEvent = H.fireEvent,
        isNumber = H.isNumber,
        isObject = H.isObject,
        offset = H.offset,
        pick = H.pick,
        splat = H.splat,
        Tooltip = H.Tooltip;

    /**
     * Set the JS DOM events on the container and document. This method should
     * contain a one-to-one assignment between methods and their handlers. Any
     * advanced logic should be moved to the handler reflecting the event's
     * name.
     *
     * @private
     * @function Highcharts.Pointer#setDOMEvents
     */
    H.Pointer.prototype.setDOMEvents = function () {

        var pointer = this,
            container = pointer.chart.container,
            ownerDoc = container.ownerDocument;

        container.onmousedown = function (e) {
            pointer.onContainerMouseDown(e);
        };
        container.onmousemove = function (e) {
            pointer.onContainerMouseMove(e);
        };
        container.onclick = function (e) {
            pointer.onContainerClick(e);
        };
        this.unbindContainerMouseLeave = addEvent(
            container,
            'mouseleave',
            pointer.onContainerMouseLeave
        );

        // CUSTOM START
        if (!H.unbindDocumentMouseUp) {
            H.unbindDocumentMouseUp = [];
        }
        H.unbindDocumentMouseUp.push(addEvent(
            ownerDoc,
            'mouseup',
            pointer.onDocumentMouseUp
        ));
        // CUSTOM END

        if (H.hasTouch) {
            container.ontouchstart = function (e) {
                pointer.onContainerTouchStart(e);
            };
            container.ontouchmove = function (e) {
                pointer.onContainerTouchMove(e);
            };
            if (!H.unbindDocumentTouchEnd) {
                H.unbindDocumentTouchEnd = addEvent(
                    ownerDoc,
                    'touchend',
                    pointer.onDocumentTouchEnd
                );
            }
        }

    };

    /**
     * Destroys the Pointer object and disconnects DOM events.
     *
     * @function Highcharts.Pointer#destroy
     */
    H.Pointer.prototype.destroy = function () {
        var pointer = this;

        if (pointer.unDocMouseMove) {
            pointer.unDocMouseMove();
        }

        this.unbindContainerMouseLeave();

        if (!H.chartCount) {
            if (H.unbindDocumentMouseUp) {
                // CUSTOM
                H.unbindDocumentMouseUp.forEach(function (unbind) {
                    unbind();
                }); // = H.unbindDocumentMouseUp();
            }


            if (H.unbindDocumentTouchEnd) {
                H.unbindDocumentTouchEnd = H.unbindDocumentTouchEnd();
            }
        }

        // memory and CPU leak
        clearInterval(pointer.tooltipTimeout);

        H.objectEach(pointer, function (val, prop) {
            pointer[prop] = null;
        });
    };

}(Highcharts));