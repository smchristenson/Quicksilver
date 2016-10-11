﻿var Checkout = {
    init: function () {
        $(document)
            .on('change', '.jsChangePayment', Checkout.changePayment)
            .on('change', '.jsChangeShipment', Checkout.changeShipment)
            .on('change', '.jsChangeAddress', Checkout.changeAddress)
            .on('change', '#MiniCart', Checkout.refreshView)
            .on('click', '.jsNewAddress', Checkout.newAddress)
            .on('click', '#AlternativeAddressButton', Checkout.enableShippingAddress)
            .on('click', '.remove-shipping-address', Checkout.removeShippingAddress)
            .on('click', '.js-add-couponcode', Checkout.addCouponCode)
            .on('click', '.js-remove-couponcode', Checkout.removeCouponCode);

        Checkout.initializeAddressAreas();
    },
    initializeAddressAreas: function () {

        if ($("#UseBillingAddressForShipment").val() == "False") {
            $("#AlternativeAddressButton").click();
        }
        else {
            $(".shipping-address").css("display", "none");
            $(".remove-shipping-address").click();
        }
    },
    addCouponCode: function (e) {
        e.preventDefault();
        var couponCode = $(inputCouponCode).val();
        var viewName = $(ViewName).val();
        if (couponCode.trim()) {
            $.ajax({
                type: "POST",
                url: $(this).data("url"),
                data: { couponCode: couponCode, viewName: viewName },
                success: function (result) {
                    if (!result) {
                        $('.couponcode-errormessage').show();
                        return;
                    }
                    $('.couponcode-errormessage').hide();
                    $("#CheckoutView").replaceWith($(result));
                    Checkout.initializeAddressAreas();
                }
            });
        }
    },
    removeCouponCode: function (e) {
        e.preventDefault();
        var viewName = $(ViewName).val();
        $.ajax({
            type: "POST",
            url: $(this).attr("href"),
            data: { couponCode: $(this).siblings().text(), viewName: viewName },
            success: function (result) {
                $("#CheckoutView").replaceWith($(result));
                Checkout.initializeAddressAreas();
            }
        });
    },
    refreshView: function () {

        var view = $("#CheckoutView");

        if (view.length == 0) {
            return;
        }
        var url = view.data('url');
        $.ajax({
            cache: false,
            type: "GET",
            url: view.data('url'),
            success: function (result) {
                view.replaceWith($(result));
                Checkout.initializeAddressAreas();
            }
        });
    },
    newAddress: function (e) {
        e.preventDefault();
        AddressBook.showNewAddressDialog($(this));
    },
    changeAddress: function () {

        var form = $('.jsCheckoutForm');
        $("#ShippingAddressIndex").val($(".jsChangeAddress").index($(this)) - 1);

        $.ajax({
            type: "POST",
            url: $(this).closest('.jsCheckoutAddress').data('url'),
            data: form.serialize(),
            success: function (result) {
                $("#AddressContainer").html($(result));
                Checkout.initializeAddressAreas();
                $.ajax({
                    type: "POST",
                    url: $('.jsOrderSummary').data('url'),
                    success: function (result) {
                        $(".jsOrderSummary").html($(result));
                    }
                });
            }
        });
    },
    changePayment: function () {
        var form = $('.jsCheckoutForm');
        $.ajax({
            type: "POST",
            url: form.data("updateurl"),
            data: form.serialize(),
            success: function (result) {
                $('.jsPaymentMethod').replaceWith($(result).find('.jsPaymentMethod'));
                Checkout.updateOrderSummary();
                Misc.updateValidation('jsCheckoutForm');
            }
        });
    },
    changeShipment: function () {
        var form = $('.jsCheckoutForm');
        $.ajax({
            type: "POST",
            url: form.data("updateurl"),
            data: form.serialize(),
            success: function (result) {
                $('.jsPaymentMethod').replaceWith($(result).find('.jsPaymentMethod'));
                Checkout.updateOrderSummary();
            }
        });
    },
    updateOrderSummary: function () {
        $.ajax({
            cache: false,
            type: "GET",
            url: $('.jsOrderSummary').data('url'),
            success: function (result) {
                $('.jsOrderSummary').replaceWith($(result).filter('.jsOrderSummary'));
            }
        });
    },
    enableShippingAddress: function (event) {

        event.preventDefault();

        var $billingShippingMethods = $(".billing-shipping-method");
        var $selectedShippingMethodId = $(".jsChangeShipment:checked", $billingShippingMethods).val();
        $("input[value='" + $selectedShippingMethodId + "']").prop('checked', true);
        $("#AlternativeAddressButton").hide();
        $(".billing-shipping-method").hide();
        $(".shipping-address:hidden").slideToggle(300);
        $("#UseBillingAddressForShipment").val("False");

    },
    removeShippingAddress: function (event) {

        event.preventDefault();

        var $billingShippingMethods = $(".billing-shipping-method");
        var $selectedShippingMethodId = $(".jsChangeShipment:checked", $(this).closest(".shipping-address")).val();
        $("#" + $selectedShippingMethodId).prop('checked', true);
        $("#AlternativeAddressButton").show();
        $(".billing-shipping-method").show();
        $(".shipping-address:visible").slideToggle(300);
        $("#UseBillingAddressForShipment").val("True");

    }
};