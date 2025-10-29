(function(global) {
    "use strict";

    function luhnCheck(number) {
        var digits = (number || "").replace(/\D/g, "");
        if (!digits) return false;
        var sum = 0;
        var shouldDouble = false;
        for (var i = digits.length - 1; i >= 0; i--) {
            var d = parseInt(digits.charAt(i), 10);
            if (isNaN(d)) return false;
            if (shouldDouble) {
                d = d * 2;
                if (d > 9) d = d - 9;
            }
            sum += d;
            shouldDouble = !shouldDouble;
        }
        return (sum % 10) === 0;
    }

    function formatCardNumber(value) {
        return (value || "").replace(/\D/g, "").slice(0, 16).replace(/(.{4})/g, "$1 ").trim();
    }

    global.luhnCheck = luhnCheck;
    global.formatCardNumber = formatCardNumber;
})(window);


