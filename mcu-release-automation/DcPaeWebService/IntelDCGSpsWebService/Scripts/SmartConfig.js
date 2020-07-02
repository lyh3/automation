function onCollapseClick(node) {
    if (node.style.display == "block") {
        node.style.display = "none";
    } else {
        node.style.display = "block";       
    }
};

function onExpendAllClick(img) {
    var containerId = "Container_{0}".format(img.id)
    var container = document.getElementById(containerId);
    if (null != container) {
        container.style.display = "block";
        RecursiveLoadSubNode(img.title);
    }
};

function RecursiveLoadSubNode(jpath) {
    if (!(null == jpath || jpath == '')) {
        if (jpath != '') {
            $.ajax({
                url: '/SmartConfig/GetSubNode',
                dataType: 'json',
                type: "POST",
                cache: false,
                data: {
                    JsonString: jpath
                },
                success: function (data) {
                    var slecter = $('#{0}'.format(subNodeId));
                    if (null != selecter) {
                        slecter.innerHTML = data.JsonString;
                    }
                    RecursiveLoadSubNode(data.SelectedNextSubNodePath);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('xhr.status : ' + xhr.status);
                }
            });
        }
    }
}

function onImgUpdaNodeClicked(img) {
    $.ajax({
        url: '/SmartConfig/UpdateNode',
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
            JsonString: img.title
        },
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
}

function onPropertyUpdaNodeClicked2(img) {
    $.ajax({
        url: '/SmartConfig/UpdateNode',
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
            JsonString: img.title
        },
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
}

function onImgEditNodeClicked(img) {
    var divedit = $("#{0}".format(img.title));
    if (divedit[0].className != '') {
        divedit.removeClass("disabled");
    } else {
        divedit.addClass("disabled");
    }
}

function onApplyClicked() {
    if (window.confirm('Are you sure to apply your changes?')) {
        $.ajax({
            url: '/SmartConfig/Apply',
            dataType: 'json',
            type: "POST",
            cache: false,
            data: {
            },
            success: function (data) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log('xhr.status : ' + xhr.status);
            }
        });
    }
}
function onRefreshClick() {
    $.ajax({
        url: '/SmartConfig/Reset',
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
        },
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
    $('#divErrorMessage').attr("style", "display:none;");
    location.reload();
};
function onSelectedSelectedSubMenuChanged(select) {
    console.log('--select id = ' + select.id);
    var value = select.value;
    console.log('--value = ' + value);
    var idDivSubNode = document.getElementById(select.title)
    console.log('--title = ' + select.title);
    if (idDivSubNode != null) {
        $.ajax({
            url: '/SmartConfig/GetSubNode',
            dataType: 'json',
            type: "POST",
            cache: false,
            data: {
                JsonString: value
            },
            success: function (data) {
                idDivSubNode.innerHTML = data.SubnodeHtml;

            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log('xhr.status : ' + xhr.status);
            }
        });
    }
}

function displayLoader(disp) {

}

function onConfigPropertyChanged(setting) {
    var value = document.getElementById(setting.id).value.trim();
    var split = setting.id.split('_');
    var msg = '-- property changed - title = {0}, value = {1}, id = {2}, split = {3}'.format(setting.title, value, setting.id, split.join(','));
    console.log(msg);
    var jsondata = '';
    var jsondataFormat_RawDataMap = '{"{0}":{"Offset":"{1}","Size":"{2}","Value":"{3}"},"Group":"{0}","Id":"{4}"}';
    var jsondataFormat_Properties = '{"{0}":{"pType":"{1}","Default":"{2}","CurrentValue":"{3}","SelectedValue":"{4}"},"Group":"{0}", "Id":"{5}"}';
    var group = split[0].trim();
    var updateContents = '';
    console.log('-- formatstring = {0}, group = {1}'.format(jsondataFormat_RawDataMap, group));
    if ('RawDataMap'.toLowerCase() == group.toLowerCase()) {
        console.log('-- jsoformat string = {0}'.format(jsondataFormat_RawDataMap));
        if (split[1].toLowerCase() == 'Offset'.toLowerCase()) {
            jsondata = jsondataFormat_RawDataMap.format(group, value, '', '', split[2]);
        } else if (split[1].toLowerCase() == 'Size'.toLowerCase()) {
            jsondata = jsondataFormat_RawDataMap.format(group, '', value, '', split[2]);
        } else if (split[1].toLowerCase() == 'Value'.toLowerCase()) {
            jsondata = jsondataFormat_RawDataMap.format(group, '', '', value, split[2]);
        } else if (split[1].toLowerCase() == 'DataSizeEdit'.toLowerCase()) {
            jsondata = jsondataFormat_RawDataMap.format(group, '', '', value, split[2]);
        } else if (split[1].toLowerCase() == 'RawDataEdit'.toLowerCase()) {
            jsondata = jsondataFormat_RawDataMap.format(group, '', '', value, split[2]);
        } else {
            jsondata = jsondataFormat_RawDataMap.format(group, '', '', '', split[2]);
        }
        updateContents = 'UpdateRawDataContents';
    };
    if (group.toLowerCase() == 'Properties'.toLowerCase()) {
        if (split[1].toLowerCase() == 'pType'.toLowerCase()) {
            jsondata = jsondataFormat_Properties.format(group, value, '', '', '', split[2]);
        } else if (split[1].toLowerCase() == 'Default'.toLowerCase()) {
            jsondata = jsondataFormat_Properties.format(group, '', value, '', '', split[2]);
        } else if (split[1].toLowerCase() == 'CurrentValue'.toLowerCase()) {
            jsondata = jsondataFormat_Properties.format(group, '', '', value, '', split[2]);
        } else if (split[1].toLowerCase() == 'Options'.toLowerCase()) {
            jsondata = jsondataFormat_Properties.format(group, '', '', '', value, split[2]);
        } else {
            jsondata = jsondataFormat_Properties.format(group, '', '', '', '', split[2]);
        }
        updateContents = 'UpdatePropertiesContents';
    };
    console.log('--- jsondata = {0}'.format(jsondata));
    $.ajax({
        url: '/SmartConfig/{0}'.format(updateContents),
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
            JsonString: jsondata
        },
        success: function (data) {
            //console.log('--- Property change update success--{0}'.format(JSON.stringify(data)));
            var container = 'Container_{0}'.format(split[2]);
            var idDivContainer = document.getElementById(container);
            if (null != idDivContainer) {
                idDivContainer.innerHTML = data.JsonString;
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
}

function onLoadBinaryClick() {
    $('#divConfigProgress').attr("style", "visibility:visible;");
};

function onRefreshClick() {
    $.ajax({
        url: '/SmartConfig/Reset',
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
        },
        success: function (data) {
            $('#divErrorMessage').attr("style", "display:none;");          
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
    location.reload();
}

function updateSmartConfigUI() {
    //document.getElementById(form).querySelectorAll("[required]")
    var guids = new Array();
    var arr = document.getElementsByClassName('smartConfig');
    Array.from(arr).forEach(function (element) {
        guids.push(element.id);
    });
    const STATUS = ['Idle', 'Modified', 'Updated', 'Applied'];
    $.ajax({
        url: '/SmartConfig/GetNodeEditStatus',
        dataType: 'json',
        type: "POST",
        cache: false,
        data: {
            JsonString: guids.join(',')
        },
        success: function (data) {
            var btnLoadBinary = $('#LoadBinary');
            var binaryInput = document.getElementById('binaryImage');
            var btnApply = $('#btnApply');
            if (null != btnLoadBinary && null != binaryInput && null != btnApply) {
                if (binaryInput.value != '') {
                    btnLoadBinary.attr("style", "visibility:visible;");
                    btnApply.attr("style", "display:none;");
                } else {
                    btnApply.attr("style", "visibility:visible;");
                }
            }
            if (null != data) {
                if (data.LastErrorMessage.trim() != '') {
                    $('#divErrorMessage').attr("style", "visibility:visible;");
                    document.getElementById('spError').innerHTML = data.LastErrorMessage;
                }
                if (data.Status.length == arr.length) {
                    var uploadBinary = $('#divTargetBinaryForm');
                    if (null != uploadBinary) {
                        if (data.ConfigLoaded) {
                            uploadBinary.removeClass('UploadBinaryDisabled');
                            uploadBinary.addClass('UploadBinaryEnabled');
                        } else {
                            uploadBinary.removeClass('UploadBinaryEnabled');
                            uploadBinary.addClass('UploadBinaryDisabled');
                        }
                    }
                    var idx = 0;
                    Array.from(arr).forEach(function (element) {
                        var rawDataTextArea = document.getElementById('RawdataMap_Value_{0}'.format(element.id));
                        var rawDataTextSelecter = $('#RawdataMap_Value_{0}'.format(element.id));
                        if (null != rawDataTextArea && null != rawDataTextSelecter) {
                            rawDataTextArea.readOnly = data.RawDataEditType[idx].toLowerCase() == 'DataSizeEdit'.toLocaleLowerCase() ? true : false;
                            rawDataTextSelecter.removeClass('rawdata');
                            rawDataTextSelecter.removeClass('rawdata_modified');
                            rawDataTextSelecter.addClass(data.RawDataClass[idx]);
                            document.getElementById('RawdataMap_Value_{0}'.format(element.id)).value = data.RawDataValue[idx];
                        }

                        document.getElementById('Properties_DefaultValue_{0}'.format(element.id)).value = data.PropertiesDefaultValue[idx];
                        document.getElementById('Properties_CurrentValue_{0}'.format(element.id)).value = data.PropertiesCurrentValue[idx];
                        //document.getElementById('RawdataMap_Offset_{0}'.format(element.id)).value = data.RawDataOffset[idx];
                        document.getElementById('RawdataMap_Size_{0}'.format(element.id)).value = data.RawDataSize[idx];

                        var selector = $('#table_{0}'.format(element.id));
                        if (null != selector) {  
                            for (var i = 0; i < STATUS.length; ++i) {
                                selector.removeClass(STATUS[i]);
                            }
                            selector.addClass(data.Status[idx]);
                            var imgUpdate = $('#imgUpdate_{0}'.format(element.id));
                            if (null != imgUpdate) {
                                imgUpdate.removeClass('updateDisabled');
                                if (data.Status[idx].toLowerCase() != 'Modified'.toLowerCase()) {
                                    imgUpdate.addClass('updateDisabled');
                                }
                            }
                        }
                        idx += 1;
                    });
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('xhr.status : ' + xhr.status);
        }
    });
    setTimeout(updateSmartConfigUI, 3000);
}

