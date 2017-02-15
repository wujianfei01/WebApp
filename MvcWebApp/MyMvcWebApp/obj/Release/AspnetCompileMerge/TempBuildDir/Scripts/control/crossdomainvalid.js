$("#bt_valid").click(
            function () {
                var myaction = $("#v_s_action").val();
                var jbox = $.jBox;
                var form = $("#f_edit");
                var close = $(parent.document.getElementById('bt_close'));
                var res = form.form('validate');
                if (res) {
                    jbox.tip("正在操作......", 'loading');
                    $.post(myaction + "?time=" + Math.random(),
                        form.serialize(),
                        function (data, status) {
                            if (status == "success") {
                                var vData = $.parseJSON(data);
                                if (vData.code == 0) {
                                    jbox.tip("操作成功!");
                                    $('#tx_save').val('1');
                                    close.click();//操作完成后關閉
                                    //form.form('reset');//操作成功后清空表單
                                }
                                else {
                                    jbox.tip("操作失敗:" + vData.msg + "!");
                                }
                            }
                            else
                                jbox.tip("操作失敗,請重試!");
                        });
                }

            });
