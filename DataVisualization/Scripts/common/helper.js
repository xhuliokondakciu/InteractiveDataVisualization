var KGT;
(function (KGT) {
    var Helper = /** @class */ (function () {
        function Helper() {
        }
        /**
         * Limit the number of calls to a function
         * @param {any} func Function to be called
         * @param {any} wait The wait time to call the function
         * @param {any} immediate If the function should be called immediately
         * @returns {Function} Debounced function
         */
        Helper.debounce = function (func, wait, immediate) {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) {
                        func.apply(context, args);
                    }
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait || 200);
                if (callNow) {
                    func.apply(context, args);
                }
            };
        };

        Helper.throttle = function (type, name, obj) {
            obj = obj || window;
            var running = false;
            var func = function () {
                if (running) { return; }
                running = true;
                requestAnimationFrame(function () {
                    obj.dispatchEvent(new CustomEvent(name));
                    running = false;
                });
            };

            obj.addEventListener(type, func);
        };

        Helper.showAlert = function (alertText, alertClass) {
            //Remove any previous alert
            let alertHtml = `<div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                                 ${alertText}
                                 <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                 </button>
                            </div>`;
            let alert = $(alertHtml);
            $("#alert-container").append(alert);
        };
        return Helper;
    }());

    KGT.Helper = Helper;
})(KGT || (KGT = {}));