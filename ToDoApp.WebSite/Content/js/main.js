var TODOLIST = {};

TODOLIST.Module = (function ($) {

    var _self = this;
    var _options = {
        doneClass: "done",
        type: "group",
        editType: false,
        todoId: 0
    }

    function init() {
        moduleCache();
        getList();
        eventListener();
    }

    function moduleCache() {
        _self.$addToListItemModal = $("#addToListItemModal");
        _self.$groupList = $("#groupList");
        _self.$marksAsDoneGroup = $("#marksAsDoneGroup");
        _self.$addItemGroup = $("#addItemGroup");
        _self.$list = $("#list");
        _self.$marksAsDoneList = $("#marksAsDoneList");
        _self.$addItemList = $("#addItemList");

        _self.$saveBtn = $("#saveBtn");

        _self.$itemName = $("#name");
        _self.$datetime = $("#datetime");
    }

    function getListTemplate(obj) {
        return '<li data-id="' + obj.Id + '">' +
          '<input type="checkbox" value="' + obj.Id + '" name="" ' + (obj.IsChecked ? 'checked' : '') + '>' +
          '<span class="text"> ' + obj.Title + '</span>' +
          '<div class="tools">' +
          '<i class="fa fa-edit"></i>' +
          '<i class="fa fa-trash-o"></i>' +
          '</div>' +
          '</li>'
    }

    function getList() {
        $.get("/todolist/get", function (data, textStatus) {
            var resultHtml = '';
            $.each(data, function (i, e) {
                resultHtml += getListTemplate(e);
            });

            var element = _self.$groupList;
            element.html(resultHtml);
            marksAsDoneProccess("group");
        }, "json");
    }

    function getTasks(id) {
        $.get("/task/get?toDoListId=" + id, function (data, textStatus) {
            var resultHtml = '';
            $.each(data, function (i, e) {
                resultHtml += getListTemplate(e);
            });

            var element = _self.$list;
            element.html(resultHtml);
            $('#listBlock').removeClass("hidden");
            marksAsDoneProccess("list");
        }, "json");
    }

    function marksAsDoneProccess(type, e) {
        if (type == "group") {
            editPostMethod({
                Id:  $(e).parent().data("id"),
                //IsChecked: self.$groupList.find("input:checked")
            });
            _self.$groupList.find("input").parent().removeClass(_options.doneClass);
            _self.$groupList.find("input:checked").parent().addClass(_options.doneClass);
            if (_self.$groupList.find("input:checked")[0] != undefined) {
                _self.$list.find("input").prop("checked", true).parent().addClass(_options.doneClass);
            } else {
                _self.$list.find("input").prop("checked", false).parent().removeClass(_options.doneClass)
            }

        } else {
            _self.$list.find("input:checked").parent().addClass(_options.doneClass);
        }
    }

    function deleteProccess($ths) {
        $ths.parent().parent().remove();

        //Buraya delete ajax method yazılabilir.
    }

    function addItemProccess() {
        if (_self.$itemName.val()) {
           var model = {
                Title: _self.$itemName.val(),
                //datetime: _self.$datetime.val()
            };

           if (_options.type == "list") {
               model.ToDoListId = _options.todoId;
           }
            addPostMethod(model);
            clearModal();
        }
    }

    function addPostMethod(model) {

        $.post(_options.type == "group" ? '/todolist/post' : '/task/post',
            model,
            function (data, status) {
                console.log("success");
                if (_options.type == "group")
                    getList();
                else
                    getTasks(_options.todoId);
                _self.$addToListItemModal.modal("hide");
            }
        );
    }

    function editProccess(type) {
        var dateElement = $(".edit").find(".label");
        var nameElement = $(".edit").find(".text");
        var date = dateElement.text();
        var name = nameElement.text();

        if (type != "post") {
            _self.$itemName.val(name);
            _self.$datetime.val(date);
        } else if (_self.$itemName.val() && _self.$datetime.val()) {
            nameElement.text(_self.$itemName.val());
            dateElement.text(_self.$datetime.val());

            editPostMethod({
                Id: "0",
                Title: _self.$itemName.val(),
                datetime: _self.$datetime.val()
            });

            clearModal();
        }
    }

    function editPostMethod(obj) {
        $(".edit").removeClass('edit');
        _self.$addToListItemModal.modal("hide");
        //buraya ajax methodu yazılabilir.
        debugger;
    }

    function clearModal() {
        _self.$itemName.val("");
        _self.$datetime.val("");
    }

    function eventListener() {
        $(document).on("change", "#groupList input", function (e) {
            marksAsDoneProccess("group", e);
        });

        $(document).on("change", "#list input", function (e) {
            marksAsDoneProccess("list", e);
        });

        _self.$addItemGroup.on("click", function () {
            _self.$addToListItemModal.modal("show");
            _options.type = "group";
            _options.editType = false;
        });

        _self.$addItemList.on("click", function () {
            _self.$addToListItemModal.modal("show");
            _options.type = "list";
            _options.editType = false;
        });

        $(document).on("click", "#groupList li", function (e) {
            $('#groupList li').removeClass("selected");
            $(this).addClass("selected");
            _options.todoId = $(this).data("id");
            getTasks($(this).data("id"));
        });

        $(document).on("click", ".fa-trash-o", function () {
            deleteProccess($(this));
        });

        $(document).on("click", ".fa-edit", function () {
            _options.editType = true;

            $(this)
              .parent()
              .parent()
              .addClass('edit');

            editProccess();
            _self.$addToListItemModal.modal("show");
        });

        _self.$saveBtn.on("click", function () {
            !_options.editType ? addItemProccess() : editProccess("post");
        });
    }

    return init;

})(jQuery);


$(function () {
    TODOLIST.Module();
    $('.datetimepicker').datetimepicker({
        locale: 'tr',
        //defaultDate: moment()
    });
});
