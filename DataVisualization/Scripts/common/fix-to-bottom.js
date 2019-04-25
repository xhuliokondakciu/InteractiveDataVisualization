(function ($) {
    /**
     * Fix a container to the bottom of the document
     * @param {any} heightFromBottom Height in pixel of gap to the bottom border
     * @returns {Array} List of jquery objects 
     */
    $.fn.fixToBottom = function (heightFromBottom) {
        const self = this;
        let defaultHeight = 5;

        KGT.Helper.throttle("resize", "optimizedResize");

        window.addEventListener("optimizedResize", function () {
            self.each(function () {
                $(this).css('height', window.innerHeight - this.offsetTop - (heightFromBottom || defaultHeight));
            });
        });

        return this.each(function () {
            $(this).css('height', window.innerHeight - this.offsetTop - (heightFromBottom || defaultHeight));
        });
    };
}(jQuery));