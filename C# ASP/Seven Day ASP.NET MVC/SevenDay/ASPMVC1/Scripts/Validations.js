// 脚本函数的声明的实现 只需要写函数
function IsFirstNameEmpty() {
    if (document.getElementById('TxtFName').value == "") {
        return 'First Name Should not be empty';
    }
    else {
        return "";
    }
}


function IsFirstNameInValid() {
    if (document.getElementById('TxtFName').value.indexOf("@")!=-1) {
        return 'First Name Should not contain @';
    }
    else {
        return "";
    }
}

function IsLastNameInValid() {
    if (document.getElementById('TxtLName').value.length>=5) {
        return 'Last Name Should not contain  more than 5 character';
    }
    else {
        return "";
    }
}

function IsSalaryEmpty() {
    if (document.getElementById('TxtSalary').value == "") {
        return 'Salary Should not be empty';
    }
    else {
        return "";
    }
}

// 返回值为布尔形但是不用声明
function IsValid() {
    var firstEmptyMessage = IsFirstNameEmpty();
    var firstValidMessage = IsFirstNameInValid();
    var lastValidMessage = IsLastNameInValid();
    var salaryEmptyMessage = IsSalaryEmpty();

    var finalErroeMessage = "Error:";
    if (firstEmptyMessage != "")
        finalErroeMessage += "\n" + firstEmptyMessage;
    if (firstValidMessage != "")
        finalErroeMessage += "\n" + firstValidMessage;
    if (lastValidMessage != "")
        finalErroeMessage += "\n" + lastValidMessage;
    if (salaryEmptyMessage != "")
        finalErroeMessage += "\n" + salaryEmptyMessage;

    if (finalErroeMessage != "Error:") {
        alert(finalErroeMessage);
        return false;
    }
    else {
        return true;
    }
}