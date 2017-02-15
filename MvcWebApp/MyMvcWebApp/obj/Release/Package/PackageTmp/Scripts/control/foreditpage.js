$(document).ready(
    function () {
        $("#bt_save").click(
            function () {
                $(window.frames["editIframe"].document).find("#bt_valid").click();

            }
            );
        $("#bt_close").click(
            function () {
                $('#edit').dialog('close');
                if ($(window.frames["editIframe"].document).find("#tx_save").val() == '1')
                    LoadDataforGrid($('#v_q_action').val());
            }
            );
    });