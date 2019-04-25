(function ($) {
    /**
     * Show loading animation on container
     * @param {string} action Action to be performend. Can be "show" or "hide"
     */
    $.fn.loading = function (action) {
        let $loadingElement = $('<div class="spinner"><div class= "bounce1" ></div><div class="bounce2"></div><div class="bounce3"></div></div>');
        switch (action) {
            case 'show':
                return this.each(function () {
                    $(this).append($loadingElement.clone());
                });
                break;
            case 'hide':
                return this.each(function () {
                    $(this).find(".spinner").remove();
                });
                break;
        }
    };
}(jQuery));