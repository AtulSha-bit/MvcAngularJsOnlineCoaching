var app = angular.module("app", ['ngFileUpload', 'ngTable', 'moment-picker', 'ngStorage', 'angucomplete-alt']);
app.controller("MyCtrl", function ($scope, $http, Upload, NgTableParams, $sessionStorage, $scope, $filter, $window) {

    $('#myModal').modal('show');
    $scope.addmulticoursetable = true;
    $scope.PaymentStatus = "Pending";
    $scope.disabledJoinDate = true;

    // Change password
    $scope.ChangePassword = function () {
        $http.post("/User/ChangePassword", { strOldPassword: $scope.mdlOldPassword, strNewPassword: $scope.mdlNewPassword }).then(function (resChangPwd) {
            if (resChangPwd.data === "OldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangPwd.data) > 0) {
                $scope.AddActivity(parseInt(resChangPwd.data), "Student", "Change Password", "Student Change Password Msg", "Student Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/User/StudentWelcome";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    };

    // Change Tutor Password

    $scope.ChangeTutorPassword = function () {
        $http.post("/User/ChangeTutorPassword", { strOldPassword: $scope.mdlOldPassword, strNewPassword: $scope.mdlNewPassword }).then(function (resChangPwd) {
            if (resChangPwd.data === "OldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangPwd.data) > 0) {
                $scope.AddActivity(parseInt(resChangPwd.data), "Tutor", "Change Password", "Tutor Change Password Msg", "Tutor Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/User/TutorPersonalProfile";
                //  window.location.href = "/User/TutorLogin";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    };

    // Change Course Student Password (Change In Code)

    $scope.ChangeCourseStudentPassword = function () {
        $http.post("/User/ChangeCourseStudentPassword", { strOldPassword: $scope.CourseStudentOldPassword, strNewPassword: $scope.CourseStudentNewPassword }).then(function (resChangCourseStudentPwd) {
            if (resChangCourseStudentPwd.data === "OldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangCourseStudentPwd.data) > 0) {
                $scope.AddActivity(parseInt(resChangCourseStudentPwd.data), "Course Student", "Change Password", "Course Student Change Password Msg", "Course Student Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/User/StudentsProfile";
                //  window.location.href = "/User/TutorLogin";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    };


    // Change Brand Student Password (Change In Code)

    $scope.ChangeBrandStudentsPassword = function () {
        $http.post("/User/ChangeBrandStudentsPassword", { strOldPassword: $scope.BrandStudentOldPassword, strNewPassword: $scope.BrandStudentNewPassword }).then(function (resChangeBrandStdPassword) {
            if (resChangeBrandStdPassword.data === "OldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangeBrandStdPassword.data) > 0) {
                $scope.AddActivity(parseInt(resChangeBrandStdPassword.data), "Brand Student", "Change Password", "Brand Student Change Password Msg", "Brand Student Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/User/StudentsProfile";
                //  window.location.href = "/User/TutorLogin";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    };



    // Change Intern Tutor Password (Change In Code)

    $scope.ChangePasswordForInternTutors = function () {
        $http.post("/InternTutor/ChangePasswordForInternTutors", { strOldPassword: $scope.InternTutorOldPassword, strNewPassword: $scope.InternTutorNewPassword }).then(function (resChangeInternTutorPassword) {
            if (resChangeInternTutorPassword.data === "OldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangeInternTutorPassword.data) > 0) {
                $scope.AddActivity(parseInt(resChangeInternTutorPassword.data), "Intern Tutor", "Change Password", "Intern Tutor Change Password Msg", "Intern Tutor Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/InternTutor/Index";
                //  window.location.href = "/User/TutorLogin";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    };



    $scope.updateConfirm = function () {
        window.location.href = "/User/ShowStudents";
    }

    $scope.enableDisableJoinDate = function () {
        if ($scope.Month !== undefined) {
            $scope.disabledJoinDate = false;
        }
        else {
            $scope.disabledJoinDate = true;
        }
    }


    $scope.arrCourse = [];

    // Add Button Add Single Or Multiple Course Validation function

    $scope.AddMultipleCourse = function () {

        if ($scope.Name === undefined || $scope.Email === undefined || $scope.Phone === undefined || $scope.State === undefined ||
            $scope.City === undefined || $scope.DOB === undefined || $scope.PinCode === undefined || $scope.Address === undefined ||
            $scope.LandMark === undefined || $scope.Course === undefined || $scope.strDuration === undefined || $scope.Fees == undefined ||
            $scope.NetAmount === undefined || $scope.Month === undefined || $scope.JoiningDate === undefined) {
            swal({
                title: 'Error',
                text: 'All fields are required',
                icon: "warning",
                dangerMode: "true"
            });
        }

        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.Phone != undefined) {
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            if (!$scope.mobilepatterns.test($scope.Phone)) {
                alert("10 digits only Mobile No must start with 6 or 7 or 8 or 9");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }

        if ($scope.State != undefined) {
            $scope.Statepattern = /^[a-z A-Z]+$/;
            if (!$scope.Statepattern.test($scope.State)) {
                alert("State must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.City != undefined) {
            $scope.Citypattern = /^[a-z A-Z]+$/;
            if (!$scope.Citypattern.test($scope.City)) {
                alert("City must contain alphabets A-Z and a-z only");
            }
        }


        if ($scope.PinCode !== undefined) {
            $scope.PinCodepattern = /^[0-9]{6,6}$/;
            if (!$scope.PinCodepattern.test($scope.PinCode)) {
                alert("6 digits only in Pin Code");
            }
        }

        var todaydate = moment();
        if (todaydate < $scope.CourseValidFrom._d && todaydate > $scope.CourseValidTo._d) {
            alert("Course expired");
        }


        if ($scope.Name != undefined && $scope.Phone != undefined && $scope.Email != undefined && $scope.State != undefined && $scope.City != undefined &&
            $scope.DOB != undefined && $scope.PinCode != undefined && $scope.Address != undefined && $scope.LandMark != undefined &&
            $scope.Course != undefined && $scope.strDuration != undefined && $scope.CourseValidFrom != undefined && $scope.CourseValidTo != undefined &&
            $scope.Fees != undefined && $scope.NetAmount != undefined && $scope.Month != undefined && $scope.JoiningDate != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            $scope.Statepattern = /^[a-z A-Z]+$/;
            $scope.Citypattern = /^[a-z A-Z]+$/;
            $scope.PinCodepattern = /^[0-9]{6,6}$/;

            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email) &&
                $scope.mobilepatterns.test($scope.Phone) && $scope.Statepattern.test($scope.State) && $scope.Citypattern.test($scope.City)
                && $scope.PinCodepattern.test($scope.PinCode)) {
                $http.get(`/User/GetCourseName?CourseId=${$scope.Course.CourseId}`).then(function (response) {
                    $scope.data = response.data;
                })

                $scope.arrCourse.push({
                    Course: $scope.Course.CourseId, CourseName: $scope.selectedCourse, Duration: $scope.strDuration, ValidFrom: $scope.mntCourseValidFrom,
                    ValidTo: $scope.mntCourseValidTo, Fees: $scope.Fees, Discount: $scope.Discount,
                    FeesWithDiscount: $scope.FeesAfterDiscount, NetAmount: $scope.NetAmount, Month: $scope.Month,
                    JoiningDate: $scope.JoiningDate, Structure: $scope.StructureId
                });

                $scope.TotalMoney = null;
                var i = 0;
                while (i < $scope.arrCourse.length) {
                    $scope.TotalMoney += $scope.arrCourse[i].NetAmount;
                    $scope.TotalMoney = Math.round($scope.TotalMoney);
                    i++;
                }

                $scope.insertStudent = true;
                $scope.disableInsertBtn = false;
                $scope.CourseValidFrom = "";
                $scope.CourseValidTo = "";
                $scope.Fees = "";
                $scope.Discount = "";
                $scope.FeeAfterDiscount = "";
                $scope.NetAmount = "";
                $scope.Month = "";
                $scope.JoiningDate = "";
                $scope.FeesAfterDiscount = "";
                $scope.hideCourseValidFrom = true;
                //$scope.hideCourseValidTo = true; by D on 30 JUne
                // $scope.hideFees = true;
                $scope.hideFees = true;
                //   $scope.hideDiscount = true;
                $scope.hideFeesAfterDiscount = true;
                //     $scope.hideNetAmount = true;
                $scope.hideMonth = true;
                $scope.hideJoiningDate = true;
                $scope.addmulticoursetable = false;
                $scope.hideAddbtn = true;
                $scope.arraydata = $scope.arrCourse;

            }
            else {
                swal({
                    title: 'Error!',
                    text: 'Please check all required fields are fill and validated',
                    icon: "warning",
                    dangerMode: "true"
                });
            }
        }

    }

    // Add Multiple Course for Student Profile Page On 13 August 2020 

    // Add Multiple Course for Student Profile Page On 13 August 2020 
    $scope.AddMultipleCourseForStudent = function () {

        //  $scope.DOB._d === undefined

        if ($scope.Course === undefined || $scope.strDuration === undefined || $scope.Fees == undefined ||
            $scope.NetAmount === undefined || $scope.Month === undefined || $scope.JoiningDate === undefined) {
            swal({
                title: 'Error',
                text: 'All fields are required',
                icon: "warning",
                dangerMode: "true"
            });
        }

        var todaydate = moment();
        if (todaydate < $scope.CourseValidFrom._d && todaydate > $scope.CourseValidTo._d) {
            alert("Course expired");
        }

        if ($scope.Course != undefined && $scope.strDuration != undefined && $scope.CourseValidFrom != undefined && $scope.CourseValidTo != undefined &&
            $scope.Fees != undefined && $scope.NetAmount != undefined && $scope.Month != undefined && $scope.JoiningDate != undefined) {

            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            //$scope.Statepattern = /^[a-z A-Z]+$/;
            //$scope.Citypattern = /^[a-z A-Z]+$/;
            //$scope.PinCodepattern = /^[0-9]{6,6}$/;
            $scope.mntCourseValidFrom = $scope.mntCourseValidFrom;
            $scope.mntCourseValidTo = $scope.mntCourseValidTo;
            $scope.arrCourse.push({
                Course: $scope.Course.CourseId, CourseName: $scope.Course.CourseName, Duration: $scope.strDuration.DurationId, DurationName: $scope.strDuration.DurationName, ValidFrom: $scope.mntCourseValidFrom,
                ValidTo: $scope.mntCourseValidTo, Fees: $scope.Fees, Discount: $scope.Discount,
                FeesWithDiscount: $scope.FeesAfterDiscount, NetAmount: $scope.NetAmount, Month: $scope.Month,
                JoiningDate: $scope.JoiningDate, StructureId: $scope.StructureId
            });

            $scope.TotalMoney = null;
            var i = 0;
            while (i < $scope.arrCourse.length) {
                $scope.TotalMoney += $scope.arrCourse[i].NetAmount;
                $scope.TotalMoney = Math.round($scope.TotalMoney);
                i++;
            }

            $scope.insertStudent = true;
            $scope.disableInsertBtn = false;
            $scope.CourseValidFrom = "";
            $scope.CourseValidTo = "";
            $scope.Fees = "";
            $scope.Discount = "";
            $scope.FeeAfterDiscount = "";
            $scope.NetAmount = "";
            $scope.Month = "";
            $scope.JoiningDate = "";
            $scope.FeesAfterDiscount = "";
            $scope.hideCourseValidFrom = true;
            $scope.hideFees = true;
            //   $scope.hideDiscount = true;
            $scope.hideFeesAfterDiscount = true;
            //     $scope.hideNetAmount = true;
            $scope.hideMonth = true;
            $scope.hideJoiningDate = true;
            $scope.hideSavebtn = false;
            $scope.addmulticoursetable = false;
            $scope.hideAddbtn = true;
            $scope.arraydata = $scope.arrCourse;
        }
        else {
            swal({
                title: 'Error!',
                text: 'Please check all required fields are fill and validated',
                icon: "warning",
                dangerMode: "true"
            });
        }
        //}
    }



    //Detelte the particular course from select course which are from student(user)

    $scope.deleteFromSelectedCourse = function (index) {
        if (index > -1) {
            $scope.NetAmountWhileDelete = $scope.arrCourse[index].NetAmount;
            $scope.data2 = $scope.TotalMoney;
            $scope.arrCourse.splice(index, 1);
            $scope.TotalMoney = $scope.data2 - $scope.NetAmountWhileDelete;
            if ($scope.arrCourse.length == 0) {
                $scope.insertStudent = false;
            }
            else {
                $scope.insertStudent = true;
            }

            console.log($scope.arrCourse.length);
        }
        //Slice or splice
    }


    $scope.callInit = function () {

        $scope.clrSession = function () {
            sessionStorage.clear();
            location.href = "/User/StudentTutorRegister";
        }

        $scope.Payment = "Cash";
        $scope.editmulticoursetable = true;

        //Calculation for due amount
        $scope.forDueAmountCalculation = function () {
            //$scope.DueAmount = $scope.NetAmount - $scope.PaidAmount;
            $scope.DueAmount = $scope.Total - $scope.PaidAmount;
        }

        $scope.validateNextInstallmentDate = function () {
        }

        $http.get('/User/GetCourses').then(function (response) {
            $scope.CourseData = response.data;
        })

        if (sessionStorage.EditData !== undefined) {
            $scope.data2 = JSON.parse(sessionStorage.EditData);
            $scope.Id = $scope.data2;
            $scope.showsendOtp = false;
            $scope.tempregno = true;
            $scope.hidebasic = false;
            $scope.coursedetails = false;
            $scope.hidecourseValidation = true;
            $scope.disableState = true;
            $scope.disableCity = true;
            $scope.disablePinCode = true;
            $scope.disableAddress = true;
            $scope.disableLandMark = true;
            $scope.disableCourse = true;
            $scope.disableDuration = true;
            $scope.disableDiscount = true;
            $scope.disableFeeAfterDiscount = true;
            $scope.disableNetAmountToPay = true;
            $scope.disableMonth = true;
            $scope.disableJoiningDate = true;
            $scope.disablePayment = true;
            $scope.disableRemarks = true;
            $scope.updateStudent = true;
            $scope.insertStudent = false;
            $scope.paymentdetails = false;
            $scope.Payment = "Cash";
            $scope.disabledJoinDate = true;
            $scope.disabledDOB = true;
            $scope.addmulticoursetable = true;
            $scope.hideAddbtn = true;
            $scope.editmulticoursetable = false;
            $scope.hideCourseDur = true;
            $scope.hideFees = true;//false => Visible
            //$scope.hideDiscount = true;
            $scope.hideFeesAfterDiscount = true; //false => Visible
            //  $scope.hideNetAmount = true;
            $scope.hideMonth = true;
            $scope.hideJoiningDate = true;
            // $scope.hideAddbtn = true;

            $http.get('/User/GetCourses').then(function (response) {
                $scope.CourseData = response.data;
            });

            $scope.Total = null;
            $http.get(`/User/EditStudents?StudentId=${$scope.Id}`).then(function (response) {
                $scope.data = response.data;
                if ($scope.data != undefined) {
                    $scope.Name = $scope.data.StudentName;
                    $scope.Phone = $scope.data.Mobile;
                    $scope.Email = $scope.data.Email;
                    $scope.State = $scope.data.State;
                    $scope.City = $scope.data.City;
                    $scope.PinCode = $scope.data.PinCode;
                    $scope.Address = $scope.data.Address;
                    $scope.LandMark = $scope.data.LandMark;
                    $scope.Course = $scope.data.Course;
                    $scope.strDuration = $scope.data.Duration;
                    $scope.mntDOB = $scope.data.DOB;
                    $scope.mntCourseValidFrom = $scope.data.ValidFrom;
                    $scope.mntCourseValidTo = $scope.data.ValidTo;
                    $scope.Fees = $scope.data.Fees;

                    //if ($scope.data.DiscountPercent != undefined || 0 || "") {
                    //    $scope.showDiscount = true;
                    //    $scope.showFeeAfterDiscount = true;
                    //    $scope.Discount = $scope.data.DiscountPercent;
                    //    $scope.FeesAfterDiscount = $scope.data.FeeAfterDiscount;
                    //} else {
                    //    $scope.showDiscount = false;
                    //    $scope.showFeeAfterDiscount = false;
                    //}

                    $scope.Discount = $scope.data.DiscountPercent;
                    $scope.FeesAfterDiscount = $scope.data.FeeAfterDiscount;

                    $scope.NetAmount = $scope.data.NetAmount;
                    $scope.Month = $scope.data.Month;
                    $scope.JoiningDate = $scope.data.JoiningDate;
                    $scope.PaymentMode = $scope.data.PaymentMode;
                    $scope.Remarks = $scope.data.Remarks;
                    $scope.TemporaryRegNo = $scope.data.TemporaryRegNo;
                    $scope.FinalRegNo = $scope.data.FinalRegNo;
                    $scope.PaymentStatus = $scope.data.PaymentStatus;
                    $scope.PaidAmount = $scope.data.PaidAmount;
                    $scope.DueAmount = $scope.data.Due;
                    $scope.dtNextInstallmentDate = $scope.data.NextInstallmentDate;
                    $scope.RemarksPayment = $scope.data.RemarksPayment;
                    $scope.hidecourseValidation = false;
                }
            });

            $http.get(`/User/EditMultipleCourseDetail?StudentId=${$scope.Id}`).then(function (response) {
                $scope.multipleCourse = response.data;
                var i = 0;
                while (i < $scope.multipleCourse.length) {
                    $scope.Total += $scope.multipleCourse[i].NetAmount;
                    $scope.Total = Math.floor($scope.Total);
                    i++;
                }
            })

        }
        else {
            $scope.insertStudent = false;
            // $scope.insertStudent = true;
            $scope.updateStudent = false;
            $scope.showsendOtp = true;
            $scope.hidebasic = true;
            $scope.coursedetails = true;
            $scope.paymentdetails = true;
            $scope.hideFees = false;
            $scope.hideFeesAfterDiscount = false;
            $scope.tempregno = false;
            $scope.hidecourseValidation = true;

            $http.get('/User/GetCourses').then(function (response) {
                $scope.CourseData = response.data;
            })

            $scope.Payment = "Cash";
        }

        $scope.emailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    }


    // Update Student detail function with validation

    $scope.updateStudentDetails = function () {

        if ($scope.Name === undefined || $scope.Email === undefined || $scope.Phone === undefined || $scope.PaymentStatus === undefined ||
            $scope.PaidAmount === undefined || $scope.FinalRegNo === undefined || null) {
            swal({
                title: 'Error',
                text: 'Name,Email,Mobile,Payment Status, Paid Amount, Final Reg No are required',
                icon: "warning",
                dangerMode: "true"
            });
        }

        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.Phone != undefined) {
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            if (!$scope.mobilepatterns.test($scope.Phone)) {
                alert("10 digits only Mobile No must start with 6 or 7 or 8 or 9");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }


        //if ($scope.PaidAmount != undefined) {
        //    $scope.PaidAmountpatterns = /^[0-9]{10}$/;
        //    if ($scope.PaidAmountpatterns.test(parseInt($scope.PaidAmount))) {
        //        alert("Paid Amount must contains digits only");
        //    }
        //}

        //  && $scope.PaidAmountpattern.test($scope.PaidAmount)



        if ($scope.Name != undefined && $scope.Email != undefined && $scope.Phone != undefined && $scope.DOB != undefined && $scope.DOB != "" &&
            $scope.PaymentStatus != undefined && $scope.PaidAmount != undefined && $scope.FinalRegNo != undefined) {

            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            $scope.PaidAmountpattern = /^[0-9]{10}$/;

            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email) &&
                $scope.mobilepatterns.test($scope.Phone)) {

                $http.post('/User/UpdateStudent', {
                    StudentId: $scope.Id,
                    Name: $scope.Name,
                    Phone: $scope.Phone,
                    Email: $scope.Email,
                    FinalRegNo: $scope.FinalRegNo,
                    DOB: moment($scope.DOB).format("YYYY-MM-DD"),
                    PaymentStatus: $scope.PaymentStatus,
                    PaidAmount: $scope.PaidAmount,
                    DueAmount: $scope.DueAmount,
                    NextInstallmentDate: moment($scope.NextInstallmentDate).format("YYYY-MM-DD"),
                    RemarksPayment: $scope.RemarksPayment,
                    ValidFrom: moment($scope.CourseValidFrom).format("YYYY-MM-DD"),
                    ValidTo: moment($scope.CourseValidTo).format("YYYY-MM-DD")

                }).then(function (response) {
                    $scope.data = response.data;

                    if (response.data == true) {
                        sessionStorage.clear();
                        $("#studentupdateModal").modal('show');
                        //alert("Student Updated Successfully");
                    }
                    else if (response.data == "Dob") {
                        swal({
                            title: 'Error',
                            text: 'Age can not be less than 3',
                            icon: "warning",
                            dangerMode: "true"
                        });
                        //  $scope.disableUpdateBtn = true;
                    }
                    else if (response.data == "nextinstallment") {
                        swal({
                            title: 'Error',
                            text: 'Next Installment Date must be between Course Validity',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }

                    else if (response.data == "NextInstallment") {
                        swal({
                            title: 'Error',
                            text: 'Next Installment Date must be greater than today',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }

                    if (response.data == false) {
                        alert("Error while updating student");
                    }
                })
            }
        }
        else {

            swal({
                title: 'Error',
                text: 'Please Check all required fields are fill and validated',
                icon: "warning",
                dangerMode: "true"
            });

            //alert("Please Check all required fields are fill and validated");
        }
    }



    // Validate date of birth at client side in Student Page

    $scope.validateDob = function () {

        if ($scope.DOB !== undefined) {
            var a = moment();
            var b = moment($scope.DOB);
            var yearsage = a.diff(b, 'years');
            var age = yearsage;

            if (age < 3) {

                // alert("Age must be greater than or equal to 18");
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 3',
                    icon: "warning",
                    dangerMode: "true"
                });
                //  $scope.regform.$invalid = true;
            }

            else {
                // $scope.regform.$invalid = false;
            }

            //else if (age >= 3 && $scope.Name !== undefined && $scope.Phone !== undefined && $scope.Email !== undefined
            //    && $scope.State !== undefined && $scope.City !== undefined && $scope.DOB !== undefined &&
            //    $scope.PinCode !== undefined && $scope.Address !== undefined && $scope.LandMark !== undefined &&
            //    $scope.Course !== undefined && $scope.strDuration !== undefined && $scope.CourseValidFrom !== undefined
            //    && $scope.CourseValidTo !== undefined && $scope.Fees !== undefined && $scope.NetAmount !== undefined &&
            //    $scope.Month !== undefined && $scope.JoiningDate !== undefined || $scope.JoiningDate != "")
            //{
            //    $scope.regform.$invalid = false;

            //}



            //if (age < 2) {
            //    $scope.regform.$invalid = false;
            //    //     $scope.registration.$invalid = false;
            //}
            //else {
            //    // alert("Age must be greater than or equal to 18");
            //    swal({
            //        title: 'Error!',
            //        text: 'Age can not be smaller than 3',
            //        icon: "warning",
            //        dangerMode: "true"
            //    });
            //    $scope.regform.$invalid = true;
            //}
        }
    }

    // Handling Joining Date and Month Validation on client side on change of joining date if joining date changed again

    $scope.getjoin = function () {

        $scope.JoiningDate = $scope.JoiningDate;
        $scope.Month = $scope.Month;

        $scope.monthpart = moment().format("MM");
        $scope.datepart = moment().format("DD");
        $scope.yearpart = moment().format("YYYY");

        if ($scope.Month == "January") {
            $scope.Month = "01";
        }
        else if ($scope.Month == "February") {
            $scope.Month = "02";
        }
        else if ($scope.Month == "March") {
            $scope.Month = "03";
        }
        else if ($scope.Month == "April") {
            $scope.Month = "04";
        }
        else if ($scope.Month == "May") {
            $scope.Month = "05";
        }
        else if ($scope.Month == "June") {
            $scope.Month = "06";
        }
        else if ($scope.Month == "July") {
            $scope.Month = "07";
        }
        else if ($scope.Month == "August") {
            $scope.Month = "08";
        }
        else if ($scope.Month == "September") {
            $scope.Month = "09";
        }
        else if ($scope.Month == "October") {
            $scope.Month = "10";
        }
        else if ($scope.Month == "November") {
            $scope.Month = "11";
        }
        else if ($scope.Month == "December") {
            $scope.Month = "12";
        }

        var date = new Date($scope.yearpart, $scope.Month - 01, $scope.JoiningDate);
        $scope.date2 = date;

        if ($scope.JoiningDate != undefined && $scope.Month != undefined &&
            $scope.date2 >= $scope.CourseValidFrom._d && $scope.date2 <= $scope.CourseValidTo._d) {

            if ($scope.Month == "01") {
                $scope.Month = "January";
            }
            else if ($scope.Month == "02") {
                $scope.Month = "February";
            }
            else if ($scope.Month == "03") {
                $scope.Month = "March";
            }
            else if ($scope.Month == "04") {
                $scope.Month = "April";
            }
            else if ($scope.Month == "05") {
                $scope.Month = "May";
            }
            else if ($scope.Month == "06") {
                $scope.Month = "June";
            }
            else if ($scope.Month == "07") {
                $scope.Month = "July";
            }
            else if ($scope.Month == "08") {
                $scope.Month = "August";
            }
            else if ($scope.Month == "09") {
                $scope.Month = "September";
            }
            else if ($scope.Month == "10") {
                $scope.Month = "October";
            }
            else if ($scope.Month == "11") {
                $scope.Month = "November";
            }
            else if ($scope.Month == "12") {
                $scope.Month = "December";
            }

            // $scope.regform.$invalid = false;
            $scope.hideAddbtn = false;
            $scope.disableAddBtn = false;
        }
        else {
            if ($scope.Month == "01") {
                $scope.Month = "January";
            }
            else if ($scope.Month == "02") {
                $scope.Month = "February";
            }
            else if ($scope.Month == "03") {
                $scope.Month = "March";
            }
            else if ($scope.Month == "04") {
                $scope.Month = "April";
            }
            else if ($scope.Month == "05") {
                $scope.Month = "May";
            }
            else if ($scope.Month == "06") {
                $scope.Month = "June";
            }
            else if ($scope.Month == "07") {
                $scope.Month = "July";
            }
            else if ($scope.Month == "08") {
                $scope.Month = "August";
            }
            else if ($scope.Month == "09") {
                $scope.Month = "September";
            }
            else if ($scope.Month == "10") {
                $scope.Month = "October";
            }
            else if ($scope.Month == "11") {
                $scope.Month = "November";
            }
            else if ($scope.Month == "12") {
                $scope.Month = "December";
            }
            alert("Joining date and Month must be between Course Validity");
            $scope.disableAddBtn = true;
            //  $scope.regform.$invalid = true;
        }

        //if ($scope.JoiningDate != undefined
        //    && $scope.JoiningDate > $scope.datepart && $scope.Month == $scope.monthpart
        //    || $scope.JoiningDate > $scope.datepart && $scope.Month > $scope.monthpart
        //    || $scope.JoiningDate == $scope.datepart && $scope.Month == $scope.monthpart
        //    || $scope.JoiningDate == $scope.datepart && $scope.Month > $scope.monthpart) {

        //    if ($scope.Month == "01") {
        //        $scope.Month = "January";
        //    }
        //    else if ($scope.Month == "02") {
        //        $scope.Month = "February";
        //    }
        //    else if ($scope.Month == "03") {
        //        $scope.Month = "March";
        //    }
        //    else if ($scope.Month == "04") {
        //        $scope.Month = "April";
        //    }
        //    else if ($scope.Month == "05") {
        //        $scope.Month = "May";
        //    }
        //    else if ($scope.Month == "06") {
        //        $scope.Month = "June";
        //    }
        //   else if ($scope.Month == "07") {
        //        $scope.Month = "July";
        //    }
        //   else if ($scope.Month == "08") {
        //        $scope.Month = "August";
        //    }
        //   else if ($scope.Month == "09") {
        //        $scope.Month = "September";
        //    }
        //   else if ($scope.Month == "10") {
        //        $scope.Month = "October";
        //    }
        //   else if ($scope.Month == "11") {
        //        $scope.Month = "November";
        //    }
        //   else if ($scope.Month == "12") {
        //        $scope.Month = "December";
        //    }
        //    $scope.regform.$invalid = false;
        //}
        //else {
        //    if ($scope.Month == "01") {
        //        $scope.Month = "January";
        //    }
        //    else if ($scope.Month == "02") {
        //        $scope.Month = "February";
        //    }
        //    else if ($scope.Month == "03") {
        //        $scope.Month = "March";
        //    }
        //    else if ($scope.Month == "04") {
        //        $scope.Month = "April";
        //    }
        //    else if ($scope.Month == "05") {
        //        $scope.Month = "May";
        //    }
        //    else if ($scope.Month == "06") {
        //        $scope.Month = "June";
        //    }
        //    else if ($scope.Month == "07") {
        //        $scope.Month = "July";
        //    }
        //    else if ($scope.Month == "08") {
        //        $scope.Month = "August";
        //    }
        //    else if ($scope.Month == "09") {
        //        $scope.Month = "September";
        //    }
        //    else if ($scope.Month == "10") {
        //        $scope.Month = "October";
        //    }
        //    else if ($scope.Month == "11") {
        //        $scope.Month = "November";
        //    }
        //    else if ($scope.Month == "12") {
        //        $scope.Month = "December";
        //    }
        //    alert("Joining Date must be greater than or equal to today date");
        //    $scope.regform.$invalid = true;
        //}



        //if ($scope.Month != undefined) {
        //    if ($scope.Month == "January") {
        //        $scope.Month = "01";
        //    }
        //   else if ($scope.Month == "February") {
        //        $scope.Month = "02";
        //    }
        //   else if ($scope.Month == "March") {
        //        $scope.Month = "03";
        //    }
        //   else if ($scope.Month == "April") {
        //        $scope.Month = "04";
        //    }
        //   else if ($scope.Month == "May") {
        //        $scope.Month = "05";
        //    }
        //   else if ($scope.Month == "June") {
        //        $scope.Month = "06";
        //    }

        //   else if ($scope.Month == "July") {
        //        $scope.Month = "07";
        //    }
        //   else if ($scope.Month == "August") {
        //        $scope.Month = "08";
        //    }
        //   else if ($scope.Month == "September") {
        //        $scope.Month = "09";
        //    }
        //   else if ($scope.Month == "October") {
        //        $scope.Month = "10";
        //    }
        //   else if ($scope.Month == "November") {
        //        $scope.Month = "11";
        //    }
        //   else if ($scope.Month == "December") {
        //        $scope.Month = "12";
        //    }

        //    $scope.monthpart = moment().format("MM");
        //    $scope.datepart = moment().format("DD");
        //    $scope.yearpart = moment().format("YYYY");

        //    if ($scope.JoiningDate != undefined
        //        && $scope.JoiningDate > $scope.datepart && $scope.Month == $scope.monthpart
        //        || $scope.JoiningDate > $scope.datepart && $scope.Month > $scope.monthpart
        //        || $scope.JoiningDate == $scope.datepart && $scope.Month == $scope.monthpart
        //        || $scope.JoiningDate == $scope.datepart && $scope.Month > $scope.monthpart) {

        //        if ($scope.Month == "01") {
        //            $scope.Month = "January";
        //        }
        //        else if ($scope.Month == "02") {
        //            $scope.Month = "February";
        //        }
        //        else if ($scope.Month == "03") {
        //            $scope.Month = "March";
        //        }
        //        else if ($scope.Month == "04") {
        //            $scope.Month = "April";
        //        }
        //        else if ($scope.Month == "05") {
        //            $scope.Month = "May";
        //        }
        //        else if ($scope.Month == "06") {
        //            $scope.Month = "June";
        //        }
        //       else if ($scope.Month == "07") {
        //            $scope.Month = "July";
        //        }
        //       else if ($scope.Month == "08") {
        //            $scope.Month = "August";
        //        }
        //       else if ($scope.Month == "09") {
        //            $scope.Month = "September";
        //        }
        //       else if ($scope.Month == "10") {
        //            $scope.Month = "October";
        //        }
        //       else if ($scope.Month == "11") {
        //            $scope.Month = "November";
        //        }
        //       else if ($scope.Month == "12") {
        //            $scope.Month = "December";
        //        }
        //        $scope.regform.$invalid = false;
        //    }
        //    else {
        //        if ($scope.Month == "01") {
        //            $scope.Month = "January";
        //        }
        //        else if ($scope.Month == "02") {
        //            $scope.Month = "February";
        //        }
        //        else if ($scope.Month == "03") {
        //            $scope.Month = "March";
        //        }
        //        else if ($scope.Month == "04") {
        //            $scope.Month = "April";
        //        }
        //        else if ($scope.Month == "05") {
        //            $scope.Month = "May";
        //        }
        //        else if ($scope.Month == "06") {
        //            $scope.Month = "June";
        //        }
        //        else if ($scope.Month == "07") {
        //            $scope.Month = "July";
        //        }
        //        else if ($scope.Month == "08") {
        //            $scope.Month = "August";
        //        }
        //        else if ($scope.Month == "09") {
        //            $scope.Month = "September";
        //        }
        //        else if ($scope.Month == "10") {
        //            $scope.Month = "October";
        //        }
        //        else if ($scope.Month == "11") {
        //            $scope.Month = "November";
        //        }
        //        else if ($scope.Month == "12") {
        //            $scope.Month = "December";
        //        }
        //        alert("Joining Date must be greater than or equal to today date");
        //        $scope.regform.$invalid = true;
        //    }
        //}
    }


    // Handling Joining Date and Month Validation on client side on selection of Month after Joining date got selected

    $scope.calcuatejoining = function () {

        $scope.Month = $scope.Month;

        if ($scope.JoiningDate != undefined) {

            $scope.JoiningDate = $scope.JoiningDate;

            $scope.monthpart = moment().format("MM");
            $scope.datepart = moment().format("DD");
            $scope.yearpart = moment().format("YYYY");

            if ($scope.Month == "January") {
                $scope.Month = "01";
            }
            else if ($scope.Month == "February") {
                $scope.Month = "02";
            }
            else if ($scope.Month == "March") {
                $scope.Month = "03";
            }
            else if ($scope.Month == "April") {
                $scope.Month = "04";
            }
            else if ($scope.Month == "May") {
                $scope.Month = "05";
            }
            else if ($scope.Month == "June") {
                $scope.Month = "06";
            }
            else if ($scope.Month == "July") {
                $scope.Month = "07";
            }
            else if ($scope.Month == "August") {
                $scope.Month = "08";
            }
            else if ($scope.Month == "September") {
                $scope.Month = "09";
            }
            else if ($scope.Month == "October") {
                $scope.Month = "10";
            }
            else if ($scope.Month == "November") {
                $scope.Month = "11";
            }
            else if ($scope.Month == "December") {
                $scope.Month = "12";
            }

            var date = new Date($scope.yearpart, $scope.Month - 01, $scope.JoiningDate);
            $scope.date2 = date;

            if ($scope.JoiningDate != undefined && $scope.Month != undefined &&
                $scope.date2 >= $scope.CourseValidFrom._d && $scope.date2 <= $scope.CourseValidTo._d) {

                if ($scope.Month == "01") {
                    $scope.Month = "January";
                }
                else if ($scope.Month == "02") {
                    $scope.Month = "February";
                }
                else if ($scope.Month == "03") {
                    $scope.Month = "March";
                }
                else if ($scope.Month == "04") {
                    $scope.Month = "April";
                }
                else if ($scope.Month == "05") {
                    $scope.Month = "May";
                }
                else if ($scope.Month == "06") {
                    $scope.Month = "June";
                }
                else if ($scope.Month == "07") {
                    $scope.Month = "July";
                }
                else if ($scope.Month == "08") {
                    $scope.Month = "August";
                }
                else if ($scope.Month == "09") {
                    $scope.Month = "September";
                }
                else if ($scope.Month == "10") {
                    $scope.Month = "October";
                }
                else if ($scope.Month == "11") {
                    $scope.Month = "November";
                }
                else if ($scope.Month == "12") {
                    $scope.Month = "December";
                }

                $scope.disableAddBtn = false;
                //   $scope.regform.$invalid = false;
            }
            else {
                if ($scope.Month == "01") {
                    $scope.Month = "January";
                }
                else if ($scope.Month == "02") {
                    $scope.Month = "February";
                }
                else if ($scope.Month == "03") {
                    $scope.Month = "March";
                }
                else if ($scope.Month == "04") {
                    $scope.Month = "April";
                }
                else if ($scope.Month == "05") {
                    $scope.Month = "May";
                }
                else if ($scope.Month == "06") {
                    $scope.Month = "June";
                }
                else if ($scope.Month == "07") {
                    $scope.Month = "July";
                }
                else if ($scope.Month == "08") {
                    $scope.Month = "August";
                }
                else if ($scope.Month == "09") {
                    $scope.Month = "September";
                }
                else if ($scope.Month == "10") {
                    $scope.Month = "October";
                }
                else if ($scope.Month == "11") {
                    $scope.Month = "November";
                }
                else if ($scope.Month == "12") {
                    $scope.Month = "December";
                }
                alert("Joining date and Month must be between Course Validity");
                $scope.disableAddBtn = true;
                //$scope.regform.$invalid = true;
            }
        }
    }


    // Get Fees etc on the student page on change of Course: On based of Course and Duration

    $scope.getCourseDetails = function () {

        if ($scope.strDuration != undefined && $scope.strDuration != "") {
            $scope.DurationName = $scope.strDuration.DurationId;


            $scope.DurationName = $scope.strDuration.DurationId;

            $scope.selectedIndex = $scope.Course;
            $scope.selectedCourse = $scope.Course.CourseName;


            $http.post('/User/GetCourseDetails',
                { Course: $scope.Course.CourseId, Duration: $scope.DurationName }).then(function (response) {
                    $scope.data = response.data;

                    if (response.data == "Duration") {
                        alert("Course With this duration not exist");
                        // $scope.hidediscount = true;
                        $scope.FeesAfterDiscount = "";
                        $scope.hidecourseValidation = true;
                    }

                    else if (response.data == "expireCourse") {
                        alert("Course expired");
                    }

                    else if (response.data != undefined) {
                        $scope.hidecourseValidation = false;
                        $scope.hideCourseValidFrom = false;

                        $scope.hideFees = false;

                        $scope.hideFeesAfterDiscount = false;

                        $scope.hideMonth = false;
                        $scope.hideJoiningDate = false;
                        $scope.hideAddbtn = false;
                        $scope.CourseValidFrom = moment(response.data.ValidFrom);
                        $scope.CourseValidTo = moment(response.data.ValidTo);
                        $scope.StructureId = response.data.Structure;
                    }

                    $scope.Duration = response.data.duration;
                    $scope.Fees = response.data.Fees;
                    $scope.NetAmount = response.data.NetAmount;
                    $scope.Discount = response.data.Discount;

                    if ($scope.Discount !== undefined && $scope.Discount !== "" && $scope.Discount != null) {
                        $scope.FeesAfterDiscount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
                        $scope.NetAmount = $scope.FeesAfterDiscount;
                    }
                })

        }
    }


    // Get Fees etc on the student page on change of Duration: On based of Course and Duration

    // Get Fees etc on the student page on change of Duration: On based of Course and Duration

    $scope.getallDetails = function () {

        if ($scope.Course != "" && $scope.Course != undefined) {


            $http.get(`/User/GetAllDetails?Course=${$scope.Course.CourseId}&Duration= ${$scope.strDuration.DurationId}`).then(function (response) {
                $scope.data = response.data;

                if (response.data == "Duration") {
                    alert("Course With this duration not exist");
                    //   $scope.hidediscount = true;
                    $scope.FeesAfterDiscount = "";
                    $scope.hidecourseValidation = true;
                }
                else if (response.data == "Courseexpire") {
                    alert("Course expired");
                }

                else if (response.data != undefined) {

                    $scope.hidecourseValidation = false;
                    $scope.CourseValidFrom = moment(response.data.ValidFrom);
                    $scope.CourseValidTo = moment(response.data.ValidTo);
                    $scope.hideCourseValidFrom = false;
                    $scope.hideFees = false;
                    $scope.hideFeesAfterDiscount = false;
                    $scope.hideMonth = false;
                    $scope.hideJoiningDate = false;
                    $scope.hideAddbtn = false;

                    $scope.StructureId = response.data.Structure;
                }

                $scope.Duration = response.data.duration;
                $scope.Fees = response.data.Fees;
                $scope.NetAmount = response.data.NetAmount;
                $scope.Discount = response.data.Discount;

                if ($scope.Discount !== undefined && $scope.Discount !== "" && $scope.Discount != null) {
                    //      $scope.hidediscount = false;
                    $scope.FeesAfterDiscount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
                    $scope.NetAmount = $scope.FeesAfterDiscount;
                }

                // Commented by Atul on 17 Aug
                // $scope.StructureId = response.data.Structure;
            })
        }
    }



    // Send Otp ( Send Otp to Student if Name,Email,Mobile no. Validated)

    $scope.sendOtp = function () {
        if ($scope.Name === undefined || $scope.Email === undefined || $scope.Phone === undefined) {
            swal({
                title: 'Error',
                text: 'All fields are required',
                icon: "warning",
                dangerMode: "true"
            });
        }

        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }
        if ($scope.Phone != undefined) {
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            if (!$scope.mobilepatterns.test($scope.Phone)) {
                alert("10 digits only Mobile No must start with 6 or 7 or 8 or 9");
            }
        }
        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }
        if ($scope.Name != undefined && $scope.Email != undefined && $scope.Phone != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;

            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email) &&
                $scope.mobilepatterns.test($scope.Phone)) {

                $http.post('/User/SendOtp', { Name: $scope.Name, Email: $scope.Email, Username: $scope.Username, UserType: "Student" }).then(function (response) {
                    $scope.data = response.data;
                    if (response.data == true) {
                        alert("Mail Sent Successfully");
                        $("#verifyEmailOtp").modal('show');
                    }
                    else if (response.data == 9) {
                        alert("Problem While Sending Email. Please Check Internet Connection");
                    }
                    else if (response.data == 10) {
                        alert("Some Unexpected error in the application");
                    }
                })
            }
        }
    }


    // Verify otp and insert student

    //$scope.ConfirmOtp = function () {
    //    $http.post('/User/ConfirmEmailOtp', { OTP: $scope.VerifyOTP }).then(function (response) {
    //        $scope.data = response.data;
    //        if (response.data == true) {
    //            alert("OTP verified successfully");
    //            $("#verifyEmailOtp").modal('hide');
    //            $scope.hidebasic = false;
    //            $scope.coursedetails = false;
    //            $scope.DisableOtpBtn = true;
    //            $scope.disableEmail = true;

    //            // Insert Student

    //            if ($scope.file !== undefined) {

    //                if (sessionStorage.BrandStructureId != undefined) {

    //                }
    //                else {

    //                }


    //                $scope.StudentProfileReg = {};
    //                $scope.StudentProfileReg.Name = $scope.Name;
    //                $scope.StudentProfileReg.DOB = moment($scope.DOB).format("YYYY-MM-DD");
    //                $scope.StudentProfileReg.ProfileImage = $scope.Title;
    //                $scope.StudentProfileReg.Fblink = $scope.Fblink;
    //                $scope.StudentProfileReg.Instalink = $scope.InstaLink;
    //                $scope.StudentProfileReg.Twitterlink = $scope.TwitterLink;
    //                $scope.StudentProfileReg.CreatedBy = "Admin";
    //                $scope.StudentProfileReg.Email = $scope.Email;
    //                $scope.StudentProfileReg.Mobile = $scope.Phone;
    //                $scope.StudentProfileReg.Username = $scope.Username;
    //                $scope.StudentProfileReg.Password = $scope.Password;
    //                $scope.StudentProfileReg.ProfileImage = $scope.FileName;
    //                $scope.StudentProfileReg.DateCreated = moment().format("YYYY-MM-DD");
    //                $scope.StudentProfileReg.IsDeleted = Boolean(0);
    //                $scope.StudentProfileReg.StudentCategoryId = $scope.StudentCategoryId;

    //                // $scope.UserRegistration.Title = $scope.file.$ngfName;
    //                Upload.upload({
    //                    url: "/User/InsertStudent",
    //                    data: {
    //                        create: $scope.StudentProfileReg,
    //                        postedfile: $scope.fullfile
    //                    }
    //                }).then(function (response) {
    //                    $scope.StudentProfileReg = response.data;

    //                    if (response.data == "Emailalready") {
    //                        swal({
    //                            title: 'Error',
    //                            text: 'Student with that email already exists',
    //                            icon: "warning",
    //                            dangerMode: "true"
    //                        });
    //                    }
    //                    else if (response.data == "Useralready") {

    //                        swal({
    //                            title: 'Error',
    //                            text: 'User with this name already exists',
    //                            icon: "warning",
    //                            dangerMode: "true"
    //                        });

    //                    }

    //                    else if (response.data == "Dob") {
    //                        swal({
    //                            title: 'Error',
    //                            text: 'Age must be greater than 18',
    //                            icon: "warning",
    //                            dangerMode: "true"
    //                        });
    //                    }
    //                    else if (parseInt(response.data) > 0) {
    //                        $scope.AddActivity(parseInt(response.data), "Student", "Insert", "Student Msg", "Student Insert Desc");
    //                        $("#insertstudentModal").modal('show');
    //                    }
    //                    else {
    //                        swal({
    //                            title: 'Error!',
    //                            text: 'Can not Insert Records! Please Contact Admin',
    //                            icon: "warning",
    //                            dangerMode: "true"
    //                        });
    //                    }
    //                })
    //            }
    //            else {
    //                swal({
    //                    title: 'Error',
    //                    text: 'File is required Please Upload Image File Only',
    //                    icon: "warning",
    //                    dangerMode: "true"
    //                })
    //            }


    //        }
    //        else {
    //            alert("Problem while verifying OTP");
    //        }
    //    })
    //}

    // Verify Otp and insert Student
    $scope.ConfirmOtp = function () {
        $http.post('/User/ConfirmEmailOtp', { OTP: $scope.VerifyOTP }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtp").modal('hide');
                $scope.hidebasic = false;
                $scope.coursedetails = false;
                $scope.DisableOtpBtn = true;
                $scope.disableEmail = true;

                // Insert Student

                //if ($scope.file !== undefined) {

                if (sessionStorage.TutorId != undefined) {

                    $scope.TutorId = JSON.parse(sessionStorage.TutorId);

                    $scope.StudentProfileReg = {};
                    $scope.StudentProfileReg.Name = $scope.Name;
                    $scope.StudentProfileReg.DOB = moment($scope.DOB).format("YYYY-MM-DD");
                    $scope.StudentProfileReg.ProfileImage = $scope.Title;
                    $scope.StudentProfileReg.Fblink = $scope.Fblink;
                    $scope.StudentProfileReg.Instalink = $scope.InstaLink;
                    $scope.StudentProfileReg.Twitterlink = $scope.TwitterLink;
                    $scope.StudentProfileReg.CreatedBy = "Admin";
                    $scope.StudentProfileReg.Email = $scope.Email;
                    $scope.StudentProfileReg.Mobile = $scope.Phone;
                    $scope.StudentProfileReg.Username = $scope.Username;
                    $scope.StudentProfileReg.Password = $scope.Password;
                    $scope.StudentProfileReg.ProfileImage = $scope.FileName;
                    $scope.StudentProfileReg.DateCreated = moment().format("YYYY-MM-DD");
                    $scope.StudentProfileReg.IsDeleted = Boolean(0);
                    $scope.StudentProfileReg.StudentCategoryId = $scope.StudentCategoryId;
                    $scope.StudentProfileReg.StudentType = "BrandStudent";

                    if ($scope.TutorId != undefined && $scope.TutorId != '') {
                        $scope.StudentProfileReg.TutorId = $scope.TutorId;
                    }

                    if (sessionStorage.BrandStructureId != undefined && sessionStorage.BrandStructureId != '') {
                        $scope.CourseStructureId = JSON.parse(sessionStorage.BrandStructureId);
                        $scope.StudentProfileReg.CourseStructureId = $scope.CourseStructureId;
                    }



                    // $scope.UserRegistration.Title = $scope.file.$ngfName;
                    Upload.upload({
                        url: "/User/InsertStudent",
                        data: {
                            create: $scope.StudentProfileReg,
                            postedfile: $scope.fullfile
                        }
                    }).then(function (response) {
                        $scope.StudentProfileReg = response.data;

                        if (response.data == "Emailalready") {
                            swal({
                                title: 'Error',
                                text: 'Student with that email already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (response.data == "Useralready") {

                            swal({
                                title: 'Error',
                                text: 'User with this name already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });

                        }

                        else if (response.data == "Dob") {
                            swal({
                                title: 'Error',
                                text: 'Age must be greater than 18',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (parseInt(response.data) > 0) {
                            sessionStorage.clear();
                            $scope.AddActivity(parseInt(response.data), "Student", "Insert", "Student Msg", "Student Insert Desc");
                            $("#insertstudentModal").modal('show');
                        }
                        else {
                            swal({
                                title: 'Error!',
                                text: 'Can not Insert Records! Please Contact Admin',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                    })
                }
                else {
                    $scope.StudentProfileReg = {};
                    $scope.StudentProfileReg.Name = $scope.Name;
                    $scope.StudentProfileReg.DOB = moment($scope.DOB).format("YYYY-MM-DD");
                    $scope.StudentProfileReg.ProfileImage = $scope.Title;
                    $scope.StudentProfileReg.Fblink = $scope.Fblink;
                    $scope.StudentProfileReg.Instalink = $scope.InstaLink;
                    $scope.StudentProfileReg.Twitterlink = $scope.TwitterLink;
                    $scope.StudentProfileReg.CreatedBy = "Admin";
                    $scope.StudentProfileReg.Email = $scope.Email;
                    $scope.StudentProfileReg.Mobile = $scope.Phone;
                    $scope.StudentProfileReg.Username = $scope.Username;
                    $scope.StudentProfileReg.Password = $scope.Password;
                    $scope.StudentProfileReg.ProfileImage = $scope.FileName;
                    $scope.StudentProfileReg.DateCreated = moment().format("YYYY-MM-DD");
                    $scope.StudentProfileReg.IsDeleted = Boolean(0);
                    $scope.StudentProfileReg.StudentCategoryId = $scope.StudentCategoryId;
                    $scope.StudentProfileReg.StudentType = "CourseStudent";

                    // $scope.UserRegistration.Title = $scope.file.$ngfName;
                    Upload.upload({
                        url: "/User/InsertStudent",
                        data: {
                            create: $scope.StudentProfileReg,
                            postedfile: $scope.fullfile
                        }
                    }).then(function (response) {
                        $scope.StudentProfileReg = response.data;

                        if (response.data == "Emailalready") {
                            swal({
                                title: 'Error',
                                text: 'Student with that email already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (response.data == "Useralready") {

                            swal({
                                title: 'Error',
                                text: 'User with this name already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });

                        }

                        else if (response.data == "Dob") {
                            swal({
                                title: 'Error',
                                text: 'Age must be greater than 18',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (parseInt(response.data) > 0) {
                            $scope.AddActivity(parseInt(response.data), "Student", "Insert", "Student Msg", "Student Insert Desc");
                            $("#insertstudentModal").modal('show');
                        }
                        else {
                            swal({
                                title: 'Error!',
                                text: 'Can not Insert Records! Please Contact Admin',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                    })
                }



                //}
                //else {
                //    swal({
                //        title: 'Error',
                //        text: 'File is required Please Upload Image File Only',
                //        icon: "warning",
                //        dangerMode: "true"
                //    })
                //}


            }
            else {
                alert("Problem while verifying OTP");
            }
        })
    }


    // Student create function
    $scope.saveStudent = function () {
        $scope.Student = {};
        // $scope.Student.CourseId = $scope.Course;
        $scope.Student.StudentName = $scope.Name;
        $scope.Student.Mobile = $scope.Phone;
        $scope.Student.Email = $scope.Email;
        $scope.Student.DOB = moment($scope.DOB).format("YYYY-MM-DD");
        $scope.Student.State = $scope.State;
        $scope.Student.City = $scope.City;
        $scope.Student.Address = $scope.Address;
        $scope.Student.LandMark = $scope.LandMark;
        $scope.Student.PinCode = $scope.PinCode;
        $scope.Student.PaymentMode = $scope.Payment;
        $scope.Student.StructureId = $scope.StructureId;
        $scope.Student.Remarks = $scope.Remarks;

        $http.post('/User/AddStudent', {

            student: $scope.Student,
            multicourse: $scope.arraydata,
            Totalmoney: $scope.TotalMoney
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                //$scope.AddActivity()
                $("#studentinsertModal").modal('show');
            }
            else if (response.data == "Dob") {
                swal({
                    title: 'Error',
                    text: 'Age can not be less than 3',
                    icon: "warning",
                    dangerMode: "true"
                });
                //  $scope.regform.$invalid = true;
            }
            else if (response.data == "discount") {
                swal({
                    title: 'Error',
                    text: 'Course expired',
                    icon: "warning",
                    dangerMode: "true"
                });
                // $scope.regform.$invalid = true;
            }
            else if (response.data == "coursevalidity") {
                swal({
                    title: 'Error',
                    text: 'Joining Date and Month must be between Course Validity',
                    icon: "warning",
                    dangerMode: "true"
                });
                //  $scope.regform.$invalid = true;
            }
            else if (response.data == "disable") {
                swal({
                    title: 'Error',
                    text: 'Joining Date and Month can not be of past date',
                    icon: "warning",
                    dangerMode: "true"
                });
                // $scope.regform.$invalid = true;
            }
            else {
                alert("Error while inserting data");
            }
        })
    }



    // Getting Courses from database on Show Student page 

    $scope.studentDetailsInit = function () {
        $http.get('/User/GetCourses').then(function (response) {
            $scope.CourseData = response.data;
        });
        $scope.hidestudentstable = true;
        $scope.PdfExport = true;
    }


    // Edit Students

    $scope.editStudentDetail = function (StudentId) {
        $scope.StudentId = StudentId;
        sessionStorage.EditData = JSON.stringify($scope.StudentId);
        window.location.href = "/User/Index";
    }


    // Getting data of students on filter based on Get Data button click on Show Students page

    $scope.getfilteredstudentsdata = function () {
        $scope.hidestudentstable = false;
        sessionStorage.EditData = null;
        sessionStorage.clear(); //Added by Dilshad on 15 June 2020
        $http.post('/User/ShowStudentDetails', {
            intCourseId: $scope.intCourseId,
            intJoiningDate: $scope.intJoiningDate,
            strMonth: $scope.strMonth
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data.length > 0) {
                $scope.Export = true;
                $scope.PdfExport = false;
            }
            else {
                $scope.Export = false;
                $scope.PdfExport = true;
            }
            $scope.StudentsDetail = new NgTableParams({}, { dataset: $scope.data });
        })
    }

    // Generate pdf of grid for student on based of filter data

    $scope.generatePDF = function (tabledata) {
        var backup = document.body.innerHTML;
        var divcontent = document.getElementById(tabledata).innerHTML;
        document.body.innerHTML = divcontent;
        window.print();
    }

    // Get Courses from database on Course Create Page

    $scope.addcourseinit = function () {

        $http.get('/Course/GetAddedCourses').then(function (response) {
            $scope.data = response.data;
            $scope.ShowUsers = new NgTableParams({}, { dataset: $scope.data });
        })
    }

    // Function for Create Course

    $scope.saveCourse = function () {

        if (sessionStorage.CourseId !== undefined && $scope.strCourse !== undefined) {
            $scope.Id = JSON.parse(sessionStorage.CourseId);
            $http.post('/Course/UpdateCourse', { intId: $scope.Id, strCourse: $scope.strCourse }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    sessionStorage.clear();
                    alert("Course Updated Successfully");
                    window.location.href = "/Course/AddCourse";
                }
                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("Course already exists");
                }
                else {
                    alert("Error! Problem while creating Course");
                }
            })
        }
        else {

            $scope.Courses = {};
            $scope.Courses.CourseName = $scope.strCourse;

            $http.post('/Course/CreateCourse', { objCourse: $scope.Courses }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    alert("Course created successfully");
                    window.location.href = "/Course/AddCourse";
                }
                else if (response.data == false) {
                    alert("Course already exists");
                }
                else if (response.data == "empty") {
                    swal({
                        title: 'Error',
                        text: 'Course is mandatory',
                        icon: "warning",
                        dangerMode: "true"
                    });
                    // alert("Please enter Course Course is mandatory");
                }

                else {
                    alert("Error! Problem while creating Course");
                }
            })
        }

    }

    // Edit Course

    $scope.editCourse = function (Id) {
        $scope.Id = Id;
        sessionStorage.CourseId = JSON.stringify($scope.Id);

        $http.get('/Course/EditCourse?Id=' + $scope.Id).then(function (response) {
            $scope.data = response.data;
            $scope.strCourse = $scope.data;
        })
    }

    // Delete Coourse get id

    $scope.deleteCourse = function (Id) {
        $scope.Id = Id;
    }

    // Edit Course Structure ( Course Details)

    $scope.editCourseDetail = function (CourseId, StructureId) {
        $scope.CourseId = CourseId;
        $scope.StructureId = StructureId;

        $http.get(`/Course/EditCourseDetail?CourseId=${$scope.CourseId}&StructureId=${$scope.StructureId}`).then(
            function (response) {
                $scope.data = response.data;
                sessionStorage.CoursesData = JSON.stringify($scope.data);
                location.href = "/Course/CourseUpdate";
            })
    }


    $scope.GetFinalAmount = function () {
        if ($scope.Fees !== undefined && $scope.DiscountPer === undefined && $scope.DiscountPer === "" &&
            $scope.DiscountPer == null && $scope.DiscountPer == 0) {
            $scope.NetAmount = $scope.Fees;
        }
        else if ($scope.Fees !== undefined && $scope.DiscountPer !== undefined) {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.DiscountPer / 100;
        }
    }

    $scope.NetAmountWithOrWithoutdiscount = function () {
        if ($scope.DiscountPer === undefined && $scope.DiscountPer == 0 && $scope.DiscountPer === "" &&
            $scope.Fees !== undefined) {
            $scope.NetAmount = $scope.Fees;
        }
        else {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.DiscountPer / 100;
        }
    }

    // Course Structure Update Initialisation

    $scope.courseUpdateInit = function () {
        $http.get('/User/GetCourses').then(function (response) {
            $scope.CourseData = response.data;
        })

        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategoryData = response.data;
        })

        $http.get('/Course/GetDuration').then(function (response) {
            $scope.DurationData = response.data;

        })

        if (sessionStorage.CoursesData !== undefined) {
            $scope.data2 = JSON.parse(sessionStorage.CoursesData);
            $scope.Course = $scope.data2.Course;
            $scope.Duration = $scope.data2.Duration;
            $scope.Month = $scope.data2.Month;
            $scope.Fees = $scope.data2.Fees;
            $scope.DiscountPer = $scope.data2.Discount;
            $scope.strDuration = $scope.data2.DurationId;
            $scope.DurName = $scope.data2.DurationName;
            $scope.StudentCategoryId = $scope.data2.StdCatgId;

            //   $scope.strDuration = $scope.data2.DurationId;

            if ($scope.DiscountPer !== undefined) {
                $scope.DiscountPer = $scope.data2.Discount;
                $scope.NetAmount = $scope.Fees - $scope.DiscountPer * $scope.Fees / 100;
            }
            else {
                $scope.NetAmount = $scope.data2.NetAmount;
            }
            $scope.ValidFrom = moment($scope.data2.ValidFrom);
            $scope.ValidTo = moment($scope.data2.ValidTo);
            $scope.StructureId = $scope.data2.StructureId;
        }
    }

    $scope.getMonths = function () {
        if ($scope.Duration == "1 Month") {
            $scope.Month = 1;
        }
        else if ($scope.Duration == "2 Months") {
            $scope.Month = 2;
        }
        else if ($scope.Duration == "3 Months") {
            $scope.Month = 3;
        }
    }

    // Update Course Structure

    $scope.updateCourseDetails = function () {
        $scope.Course = $scope.Course;
        $scope.Duration = $scope.Duration;
        $scope.Month = $scope.Month;
        $scope.Fees = $scope.Fees;
        $scope.DiscountPer = $scope.DiscountPer;
        $scope.NetAmount = $scope.NetAmount;
        $scope.ValidFrom = moment($scope.ValidFrom).format("YYYY-MM-DD");
        $scope.ValidTo = moment($scope.ValidTo).format("YYYY-MM-DD");
        $scope.StructureId = $scope.StructureId;
        $scope.strDuration = $scope.strDuration;
        $scope.DurName = $scope.DurName;
        $scope.StdCatgId = $scope.StudentCategoryId;


        $http.post('/Course/UpdateCourseDetailsMaster', {
            Course: $scope.Course,
            Fees: $scope.Fees, Discount: $scope.DiscountPer, NetAmount: $scope.NetAmount,
            ValidFrom: $scope.ValidFrom,
            ValidTo: $scope.ValidTo, StructureId: $scope.StructureId, DurationId: $scope.strDuration, DurationName: $scope.DurName, StudentCategoryId: $scope.StdCatgId
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                sessionStorage.clear();
                alert("Course Details Updated Successfully");
                window.location.href = "/Course/ShowCourseStructure";
            }
            else if (response.data == "Duration") {
                // sessionStorage.clear();
                alert("Can not update Duration");
                // alert("Course with this Duration already exists can not update");
            }
            else if (response.data == false) {
                //  sessionStorage.clear();
                alert("Problem while updating course details");
            }
            else if (response.data == "ValidTo") {
                swal({
                    title: 'Error!',
                    text: 'ValidTo must be greater than or equal to Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                //alert("ValidTo must be greater than Valid From");
                $scope.coursedetails.$invalid = true;
            }
        });
    }


    // Course Update Page Valid from validation

    $scope.validateFromDate = function () {
        if ($scope.ValidFrom !== undefined && sessionStorage.CoursesData == null) {
            $scope.ValidFromDate = moment($scope.ValidFrom).format("YYYY-MM-DD");
            sessionStorage.validfromdate = JSON.stringify($scope.ValidFromDate);
        }
        else if ($Scope.ValidFrom !== undefined && sessionStorage.CoursesData !== undefined &&
            sessionStorage.CoursesData !== "") {
            $scope.ValidFromDate = moment($scope.ValidFrom).format("YYYY-MM-DD");
            sessionStorage.validfromdate = JSON.stringify($scope.ValidFromDate);
        }
    }

    // Course Update Page Valid To validation

    $scope.ValidateToDate = function () {
        if (sessionStorage.validfromdate != null && $scope.ValidTo !== undefined && sessionStorage.CoursesData == null) {
            $scope.From = JSON.parse(sessionStorage.validfromdate);
            $scope.To = moment($scope.ValidTo).format("YYYY-MM-DD");

            if ($scope.From >= $scope.To) {
                //   alert("End Date must be greater than start date");
                swal({
                    title: 'Error!',
                    text: 'Valid To must be greater than Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                $scope.coursedetails.$invalid = true;
            }
            else {
                $scope.coursedetails.$invalid = false;

            }
        }
        else if (sessionStorage.validfromdate != null && $scope.ValidTo !== undefined &&
            sessionStorage.CoursesData !== undefined && sessionStorage.CoursesData !== "") {
            $scope.From = JSON.parse(sessionStorage.validfromdate);
            $scope.To = moment($scope.ValidTo).format("YYYY-MM-DD");

            if ($scope.From >= $scope.To) {
                //   alert("End Date must be greater than start date");
                swal({
                    title: 'Error!',
                    text: 'Valid To must be greater than Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                $scope.coursedetails.$invalid = true;
            }
            else {
                $scope.coursedetails.$invalid = false;
            }
        }
    }

    // Delete Course 

    $scope.DeleteCourse = function (Id) {
        $http.post('/Course/DeleteCourse', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Data Deleted Successfully");
                window.location.href = "/Course/AddCourse";
            }
            else if (response.data == "Coursetaken") {
                alert("Can not delete course due to dependency");
                $("#myModal").modal('hide');
            }
            else if (response.data == "str") {
                alert("Can not delete course due to depedency");
                $("#myModal").modal('hide');
            }
        })
    }


    // Init of Create Course Structure

    $scope.courseDetailInit = function () {
        $http.get('/User/GetCourses').then(function (response) {
            $scope.CourseData = response.data;
        })

        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategoryData = response.data;
        })


        //  Tutor Data
        //$http.get("/Admin/GetTutorsData").then(function (response) {
        //    $scope.TutorDataBind = response.data;
        //})

        // Brand Tutor Data

        $http.get('/User/BrandTutorData').then(function (response) {
            $scope.BrandTutorData = response.data;
        })


        // 5 September 2020 Atul Bind Duration

        $http.get('/Course/GetDuration').then(function (response) {
            $scope.DurationData = response.data;

        })

        $http.get('/Course/ShowCourses').then(function (response) {
            $scope.data = response.data;
            $scope.CourseDetail = new NgTableParams({}, { dataset: $scope.data });

            //   $scope.datas = response.data;
            //   $scope.CourseDetail = new NgTableParams({}, { dataset: $scope.datas });
        })
    }

    // Validation for Create Course Structure

    // Get Duration on based of Month

    $scope.getMonth = function () {
        if ($scope.Duration == "1 Month") {
            $scope.Month = 1;
        }
        else if ($scope.Duration == "2 Months") {
            $scope.Month = 2;
        }
        else if ($scope.Duration == "3 Months") {
            $scope.Month = 3;
        }
    }

    // Get Net Amount

    $scope.getNetAmount = function () {
        if ($scope.Discount == undefined && $scope.Fees != undefined) {
            $scope.NetAmount = $scope.Fees;
        }
        else if ($scope.Discount !== undefined && $scope.Fees !== undefined) {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
        }
    }


    // Net Amount With Discount

    $scope.netamountwithdiscount = function () {

        if ($scope.Discount === undefined && $scope.Discount == 0 && $scope.Discount === "" &&
            $scope.Fees !== undefined) {
            $scope.NetAmount = $scope.Fees;
        }
        else {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
        }
    }

    // Create Course Structure Valid From Validation

    $scope.validateFromDate = function () {
        if ($scope.ValidFrom !== undefined) {
            $scope.ValidFromDate = moment($scope.ValidFrom).format("YYYY-MM-DD");
            sessionStorage.validfromdate = JSON.stringify($scope.ValidFromDate);
        }
    }

    // Create Course Structure Valid To Validation

    $scope.ValidateToDate = function () {
        if (sessionStorage.validfromdate != null && $scope.ValidTo !== undefined) {
            $scope.From = JSON.parse(sessionStorage.validfromdate);
            $scope.To = moment($scope.ValidTo).format("YYYY-MM-DD");

            if ($scope.From >= $scope.To) {
                //   alert("End Date must be greater than start date");
                swal({
                    title: 'Error!',
                    text: 'Valid To must be greater than Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                $scope.coursedetails.$invalid = true;
            }
            else {
                $scope.coursedetails.$invalid = false;
            }
        }
    }


    // Create Course Structure

    $scope.saveCourseDetails = function () {
        $scope.CourseDetails = {};
        $scope.CourseDetails.CourseId = $scope.Course;
        $scope.CourseDetails.DurationName = $scope.Duration;
        $scope.CourseDetails.Months = $scope.Month;
        $scope.CourseDetails.Fees = $scope.Fees;
        $scope.CourseDetails.DiscountPercent = $scope.Discount;
        $scope.CourseDetails.NetAmount = $scope.NetAmount;
        $scope.CourseDetails.ValidFrom = moment($scope.ValidFrom).format("YYYY-MM-DD");
        $scope.CourseDetails.ValidTo = moment($scope.ValidTo).format("YYYY-MM-DD");
        $scope.CourseDetails.DurationId = $scope.strDuration.DurationId;
        $scope.CourseDetails.StdCatgId = $scope.StudentCategoryId;
        $scope.CourseDetails.TutorId = $scope.mdlTutorId;

        $http.post('/Course/AddCourseStructure', { courseStructure: $scope.CourseDetails }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                alert("Course structure created successfully");
                location.href = "/Course/ShowCourseStructure";
            }
            else if (response.data == false) {
                alert("Course Structure with this Duration already exists");
            }
            else if (response.data == "Error") {
                alert("Error can't insert Course Structure");
            }
        })
    }


    // Delete Course Structure Section

    // Delete Course Structure: Getting Course Id and StructureId

    $scope.deleteCourseStructure = function (CourseId, StructureId) {
        $scope.Id = CourseId;
        $scope.StructureId = StructureId;
    }

    // Delete Course Structure

    $scope.DeleteCourseStructure = function () {

        $http.post('/Course/DeleteCourseStructure', {
            Id: $scope.Id,
            StructureId: $scope.StructureId
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Data Deleted Successfully");
                window.location.href = "/Course/ShowCourseStructure";
            }
            else if (response.data = "Studentexists") {
                alert("Can not delete Course Structure due to dependency");
                $("#deleteModal").modal('hide');
            }
            else if (response.data == "strmsg") {
                alert("Some Errors occured while deleting Course Structure");
            }

            //else {
            //    alert("Error while deleting data");
            //}
        })
    }


    // Image

    $scope.upload = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;

        if (sessionStorage.TutorData != null && $scope.Title !== undefined) {
            // $scope.Title = $scope.FileName;

            var files = $scope.fullfile;
            var title = files.$ngfName;
            var reader = new FileReader();
            if (files) {
                reader.readAsDataURL(files);
                var name = files.name;
            }

            reader.addEventListener("load", function () {
                // $scope.Preview = reader.result;
                $scope.Preview = reader.result;
                $scope.ImgOnlyOnEdit = false;
                $scope.PreviewImage = true;

            }, false);

            if ($scope.Preview !== undefined) {
                $scope.Preview = name;
            }
        }
    }


    $scope.callTutorInit = function () {
        $http.get('/Admin/GetTutors').then(function (response) {
            $scope.data = response.data;

            //if ($scope.data.DoucmentNo != '' && $scope.data.DocumentNo != undefined) {
            //    $scope.Docno = $scope.data.DoucmentNo;
            //}


            $scope.data2 = [];
            $scope.data3 = [];
            $scope.data4 = [];
            for (var i = 0; i < $scope.data.length; i++) {
                $scope.data2 = $scope.data[i].DocumentNo;
                $scope.data3 = $scope.data[i].DocumentName;
                $scope.data4 = $scope.data[i].DocumentId;

                $scope.data[i].DocumentNo = $scope.data2[0];
                $scope.data[i].DocumentName = $scope.data2[1];
                $scope.data[i].DocumentId = $scope.data4;
            }
            $scope.ShowTutors = new NgTableParams({}, { dataset: $scope.data });
        })
    }

    $scope.deleteTutor = function (Id) {
        $scope.Id = Id;
    }


    $scope.DeleteTutors = function (Id) {
        $http.post('/Admin/DeleteTutors', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Data Deleted Successfully");
                location.href = "/Admin/ShowTutors";
            }
            else {
                alert("Error while deleting data");
            }
        })
    }



    $scope.tutorinit = function () {
        $scope.emailrege = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        if (sessionStorage.TutorData != undefined) {
            $scope.InsertBtn = false;
            $scope.UpdateBtn = true;
            $scope.ImgOnlyOnEdit = true;
            $scope.data = JSON.parse(sessionStorage.TutorData);
            $scope.Id = $scope.data.TutorId;
            $scope.Name = $scope.data.TutorName;
            //  $scope.mdlShortDesc = $scope.data.ShortDescription;

            if ($scope.data.ShortDescription != "null" && $scope.data.ShortDescription != "") {
                $scope.mdlShortDesc = $scope.data.ShortDescription;
            }

            // $scope.mdlShortDesc = $scope.data.ShortDescription;

            $scope.mdlLongDesc = $scope.data.LongDescription;
            $scope.Experience = $scope.data.TutorExperience;
            $scope.Contact = $scope.data.TutorContact;
            $scope.Email = $scope.data.TutorEmail;
            $scope.TutorType = $scope.data.TutorType;

            if ($scope.data.AdminDescription != '' && $scope.data.AdminDescription === undefined) {
                $scope.AdminDescription = $scope.data.AdminDescription;
            }
            $scope.momentDOB = $scope.data.TutorDOB;
            $scope.AadharNo = $scope.data.DocumentNo;
            $scope.PanNo = $scope.data.DocumentName;
            $scope.Title = $scope.data.TutorImage;
        }
        else {

            $http.get('/Admin/GetTutorCode').then(function (response) {
                $scope.TutorCode = response.data;
            })

            $scope.InsertBtn = true;
            $scope.UpdateBtn = false;
        }
    }


    $scope.validateTutorDOB = function () {

        if ($scope.DOB !== undefined) {
            var a = moment();
            var b = moment($scope.DOB);
            var yearsage = a.diff(b, 'years');
            var age = yearsage;
            if (age < 18) {
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 18',
                    icon: "warning",
                    dangerMode: "true"
                })
                //$scope.registration.$invalid = true;
            }


            else {

            }
        }
    }


    // Edit Tutor

    $scope.editTutorDetail = function (TutorSingleData) {
        sessionStorage.TutorData = JSON.stringify(TutorSingleData);
        location.href = "/Admin/Tutor";
    }

    $scope.documentarr = [];
    $scope.UpdateTutor = function () {
        $scope.TutorRegistration = {};
        $scope.Document = [];
        if (sessionStorage.TutorData !== '' && sessionStorage.TutorData != undefined) {
            var d = sessionStorage.TutorData;
            d = JSON.parse(d);
            $scope.TutorRegistration.TutorId = d.TutorId;
            $scope.Document = d.DocumentId;

            $scope.AadharDocId = d.DocumentId[0];
            $scope.PanDocId = d.DocumentId[1];
        }

        if ($scope.AadharNo != '' && $scope.AadharNo != undefined) {
            $scope.documentarr.push({ DocumentName: "AadharNumber", DoucmentNo: $scope.AadharNo, DocumentId: $scope.AadharDocId });
        }

        if ($scope.PanNo != '' && $scope.PanNo != undefined) {
            $scope.documentarr.push({ DocumentName: "PanNumber", DoucmentNo: $scope.PanNo, DocumentId: $scope.PanDocId });
        }

        $scope.TutorRegistration.TutorName = $scope.Name;
        $scope.TutorRegistration.TutorContact = $scope.Contact;
        $scope.TutorRegistration.TutorExperience = $scope.Experience;
        $scope.momentDOB = moment($scope.DOB, "dd/MM/yyyy");
        $scope.TutorRegistration.TutorDOB = $scope.momentDOB.toDate();
        $scope.TutorRegistration.TutorEmail = $scope.Email;
        $scope.TutorRegistration.TutorImage = $scope.FileName;
        $scope.TutorRegistration.ShortDescription = $scope.mdlShortDesc;
        $scope.TutorRegistration.LongDescription = $scope.mdlLongDesc;
        $scope.TutorRegistration.AdminDescription = $scope.AdminDescription;
        $scope.TutorRegistration.TutorType = $scope.TutorType;

        Upload.upload({
            url: "/Admin/UpdateTutors",
            data: {
                objTutor: $scope.TutorRegistration, postedfile: $scope.fullfile, lstMultidocument: $scope.documentarr
            }
        }).then(function (response) {
            $scope.data = response.data;
            $scope.showLoader = false;
            if (response.data == "Updated") {
                $scope.AddActivity(d.TutorId, "Tutor", "Update", "Tutor Update By Admin Msg", "Tutor Update By Admin Desc");
                sessionStorage.clear();
                $("#updateModal").modal('show');
            }

            else if (response.data == "DateBirth") {
                swal({
                    title: 'Error!',
                    text: 'Age can not be less than 18',
                    icon: "warning",
                    dangerMode: "true"
                });
            }

            else {
                // alert("Problem while updating data");

                swal({
                    title: 'Error!',
                    text: 'Can not Update Records! Please Contact Admin',
                    icon: "warning",
                    dangerMode: "true"
                });
            }
        })
    }

    // Insert Tutor

    $scope.InsertTutor = function () {
        if ($scope.file !== undefined) {
            $scope.TutorRegistration = {};
            $scope.TutorRegistration.TutorName = $scope.Name;
            $scope.TutorRegistration.TutorContact = $scope.Contact;
            $scope.TutorRegistration.TutorExperience = $scope.Experience;
            $scope.TutorRegistration.TutorDOB = moment($scope.DOB).format("YYYY-MM-DD");
            $scope.TutorRegistration.TutorDescription = $scope.Description;
            $scope.TutorRegistration.TutorEmail = $scope.Email;
            $scope.TutorRegistration.TutorImage = $scope.FileName;
            //$scope.TutorRegistration.CreatedBy = "Atul Sharma";
            //$scope.TutorRegistration.DateCreated = moment().format("YYYY-MM-DD");
            //$scope.TutorRegistration.IsDeleted = false;
            $scope.TutorRegistration.ShortDescription = $scope.mdlShortDesc;
            $scope.TutorRegistration.LongDescription = $scope.mdlLongDesc;
            $scope.TutorRegistration.TutorType = $scope.TutorType;
            // $scope.UserRegistration.Title = $scope.file.$ngfName;

            Upload.upload({
                url: "/Admin/InsertTutors",
                data: {
                    create: $scope.TutorRegistration,
                    postedfile: $scope.fullfile
                }
            }).then(function (response) {
                $scope.UserRegistration = response.data;
                if (response.data == "EndDate") {
                    swal({
                        title: 'Error',
                        text: 'End Date must be greater than start date',
                        icon: "warning",
                        dangerMode: "true"
                    });
                }
                else if (response.data == "Dob") {
                    swal({
                        title: 'Error',
                        text: 'Age must be greater than 18',
                        icon: "warning",
                        dangerMode: "true"
                    });
                }
                else if (response.data == "Insert") {

                    // alert("Data Inserted Successfully");
                    $("#inserttutorModal").modal('show');
                    // location.href = "/Home/Index";
                }
                else {
                    swal({
                        title: 'Error!',
                        text: 'Can not Insert Records! Please Contact Admin',
                        icon: "warning",
                        dangerMode: "true"
                    });

                    // alert("Error while inserting data");
                }
            })
        }

        else {
            swal({
                title: 'Error',
                text: 'File is required Please Upload Image File Only',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    // Student Register for profile Init 

    $scope.studentinit = function () {
        $scope.InsertBtn = true;
        $scope.emailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        $http.get('/User/GetStudentCode').then(function (response) {
            $scope.StudentCode = response.data;
        })

        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategoryData = response.data;
        })
    }


    // Student already exists ( When student creates itself)

    $scope.StudentExistsOrNot = function () {
        $scope.Username = $scope.Username;
        if ($scope.Username != '' && $scope.Username != undefined) {
            $http.post('/User/StudentExistsOrNot', { Name: $scope.Username }).then(function (response) {
                $scope.StudentData = response.data;
                if (response.data == true) {
                    // alert("Username already exists");
                    swal({
                        title: 'Error',
                        text: 'User with this name already exists',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
            })
        }

        else {
            swal({
                title: 'Error',
                text: 'Username is required',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    $scope.uploadImg = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;
    }


    // Send mail to student after register

    $scope.SendOtpToStudent = function () {

        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }


        if ($scope.Name != undefined && $scope.Email != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email)) {
                $http.post('/User/SendOtp', { Name: $scope.Name, Email: $scope.Email, Username: $scope.Username, UserType: "Student" }).then(function (response) {
                    $scope.data = response.data;
                    if (response.data == true) {
                        alert("Mail Sent Successfully");
                        $("#verifyEmailOtp").modal('show');
                    }
                    else if (response.data == "Useralready") {
                        swal({
                            title: 'Error!',
                            text: 'User with this name already exists',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }
                    else if (response.data == "Emailalready") {
                        swal({
                            title: 'Error!',
                            text: 'User with this Email already exists',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }

                    else if (response.data == 9) {
                        alert("Problem While Sending Email. Please Check Internet Connection");
                    }
                    else if (response.data == 10) {
                        alert("Some Unexpected error in the application");
                    }
                })
            }
        }


        else if (sessionStorage.TutorId != undefined) {

            $scope.TutorId = JSON.parse(sessionStorage.TutorId);

            if (sessionStorage.BrandStructureId != undefined && sessionStorage.BrandStructureId != '') {
                $scope.CourseStructureId = JSON.parse(sessionStorage.BrandStructureId);
            }
            if ($scope.Name != undefined && $scope.Email != undefined) {
                $scope.Namepattern = /^[a-z A-Z]+$/;
                $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
                if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email)) {
                    $http.post('/User/SendOtp', { Name: $scope.Name, Email: $scope.Email, Username: $scope.Username, UserType: "Student", TutorId: $scope.TutorId, StructureId: $scope.CourseStructureId }).then(function (response) {
                        $scope.data = response.data;
                        if (response.data == true) {
                            alert("Mail Sent Successfully");
                            $("#verifyEmailOtp").modal('show');
                        }
                        else if (response.data == "Useralready") {
                            swal({
                                title: 'Error!',
                                text: 'User with this name already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (response.data == "Emailalready") {
                            swal({
                                title: 'Error!',
                                text: 'User with this Email already exists',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }

                        else if (response.data == 9) {
                            alert("Problem While Sending Email. Please Check Internet Connection");
                        }
                        else if (response.data == 10) {
                            alert("Some Unexpected error in the application");
                        }
                    })
                }
            }
        }



    }



    // Insert Student

    $scope.InsertStudent = function () {

        if ($scope.file !== undefined) {
            $scope.StudentProfileReg = {};

            //  $scope.StudentProfileReg
            $scope.StudentProfileReg.Name = $scope.Name;
            $scope.StudentProfileReg.DOB = moment($scope.DOB).format("YYYY-MM-DD");
            $scope.StudentProfileReg.ProfileImage = $scope.Title;
            $scope.StudentProfileReg.Fblink = $scope.Fblink;
            $scope.StudentProfileReg.Instalink = $scope.InstaLink;
            $scope.StudentProfileReg.Twitterlink = $scope.TwitterLink;
            $scope.StudentProfileReg.CreatedBy = "Atul Sharma";
            $scope.StudentProfileReg.Email = $scope.Email;
            $scope.StudentProfileReg.Username = $scope.Username;
            $scope.StudentProfileReg.Password = $scope.Password;
            $scope.StudentProfileReg.ProfileImage = $scope.FileName;
            $scope.StudentProfileReg.DateCreated = moment().format("YYYY-MM-DD");
            $scope.StudentProfileReg.IsDeleted = Boolean(0);

            // $scope.UserRegistration.Title = $scope.file.$ngfName;

            Upload.upload({
                url: "/Admin/InsertStudents",
                data: {
                    create: $scope.StudentProfileReg,
                    postedfile: $scope.fullfile
                }
            }).then(function (response) {
                $scope.StudentProfileReg = response.data;


                if (response.data == "Dob") {
                    swal({
                        title: 'Error',
                        text: 'Age must be greater than 18',
                        icon: "warning",
                        dangerMode: "true"
                    });
                }

                else if (response.data == "Insert") {

                    // alert("Data Inserted Successfully");
                    $("#insertstudentModal").modal('show');
                    // location.href = "/Home/Index";
                }
                else {
                    swal({
                        title: 'Error!',
                        text: 'Can not Insert Records! Please Contact Admin',
                        icon: "warning",
                        dangerMode: "true"
                    });
                    // alert("Error while inserting data");
                }

            })
        }

        else {
            swal({
                title: 'Error',
                text: 'File is required Please Upload Image File Only',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    // Verify Tutor email and send mail ( Tutor created by tutor itself )

    $scope.SendOtpToTutor = function () {
        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }

        if ($scope.Contact != undefined) {
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            if (!$scope.mobilepatterns.test($scope.Contact)) {
                alert("10 digits only Mobile No must start with 6 or 7 or 8 or 9");
            }
        }

        if ($scope.Name != undefined && $scope.Email != undefined && $scope.Contact != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;

            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email) &&
                $scope.mobilepatterns.test($scope.Contact)) {
                $http.post('/User/SendOtp', { Name: $scope.Name, Email: $scope.Email, UserType: "Tutor" }).then(function (response) {
                    $scope.data = response.data;
                    if (response.data == true) {
                        alert("Mail Sent Successfully");
                        $("#verifyEmailOtp").modal('show');
                    }
                    else if (response.data == "Emailalreadyexists") {
                        swal({
                            title: 'Error!',
                            text: 'User with this Email already exists',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }

                    else if (response.data == 9) {
                        alert("Problem While Sending Email. Please Check Internet Connection");
                    }
                    else if (response.data == 10) {
                        alert("Some Unexpected error in the application");
                    }
                })
            }
        }
    }


    // Verify Otp  and  register Tutor ( When tutor registers itself)

    $scope.ConfirmOtpForTutor = function () {
        $http.post('/User/ConfirmEmailOtp', { OTP: $scope.VerifyOTP }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtp").modal('hide');
                $scope.hidebasic = false;
                $scope.coursedetails = false;
                $scope.DisableOtpBtn = true;
                $scope.disableEmail = true;

                // Insert Student

                if ($scope.file !== undefined) {
                    $scope.TutorRegistration = {};
                    $scope.TutorRegistration.TutorName = $scope.Name;
                    $scope.TutorRegistration.TutorContact = $scope.Contact;
                    $scope.TutorRegistration.TutorExperience = $scope.Experience;
                    $scope.TutorRegistration.TutorDOB = moment($scope.DOB).format("YYYY-MM-DD");
                    $scope.TutorRegistration.TutorDescription = $scope.Description;
                    $scope.TutorRegistration.TutorEmail = $scope.Email;
                    $scope.TutorRegistration.TutorImage = $scope.FileName;
                    $scope.TutorRegistration.CreatedBy = "Admin";
                    $scope.TutorRegistration.DateCreated = moment().format("YYYY-MM-DD");
                    $scope.TutorRegistration.IsDeleted = false;
                    $scope.TutorRegistration.ShortDescription = $scope.mdlShortDesc;
                    $scope.TutorRegistration.LongDescription = $scope.mdlLongDesc;

                    // $scope.UserRegistration.Title = $scope.file.$ngfName;

                    Upload.upload({
                        url: "/User/InsertTutor",
                        data: {
                            create: $scope.TutorRegistration,
                            postedfile: $scope.fullfile
                        }
                    }).then(function (response) {
                        $scope.UserRegistration = response.data;

                        if (response.data == "Dob") {
                            swal({
                                title: 'Error',
                                text: 'Age must be greater than 18',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                        else if (parseInt(response.data) > 0) {
                            $scope.AddActivity(parseInt(response.data), "Tutor", "Insert", "Tutor Insert Msg", "Tutor Inser Desc");
                            $("#inserttutorModal").modal('show');
                        }
                        else {
                            swal({
                                title: 'Error!',
                                text: 'Can not Insert Records! Please Contact Admin',
                                icon: "warning",
                                dangerMode: "true"
                            });
                        }
                    })
                }
                else {
                    swal({
                        title: 'Error',
                        text: 'File is required Please Upload Image File Only',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
            }
            else {
                alert("Problem while verifying OTP");
            }
        })
    }


    $scope.validateStudentDOB = function () {

        if ($scope.DOB !== undefined) {
            var a = moment();
            var b = moment($scope.DOB);
            var yearsage = a.diff(b, 'years');
            var age = yearsage;
            if (age < 18) {
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 18',
                    icon: "warning",
                    dangerMode: "true"
                })
                //$scope.registration.$invalid = true;
            }
            else {

            }
        }

    }


    // Student profile

    $scope.studentprofileinit = function () {

        $http.get('/User/ShowStudentData').then(function (response) {
            $scope.data = response.data;

            $http.get('/User/GetCourses').then(function (response) {
                $scope.CourseData = response.data;
            })

            $http.get('/User/StudentCategoryData').then(function (response) {
                $scope.StudentCategoryData = response.data;
            })


            //$http.get('/User/BrandStudentCommunicationToTutor').then(function (response) {
            //    $scope.StudentsBrandTutorData = response.data;
            //})



            $scope.StudentCategoryId = $scope.data[0].StudentCategoryId;

            $scope.Name = $scope.data[0].Name;
            $scope.Username = $scope.data[0].Username;
            $scope.Password = $scope.data[0].Password;
            $scope.momentDOB = moment($scope.data[0].DOB).format("DD/MM/YYYY");
            $scope.Title = $scope.data[0].ProfileImage;
            $scope.Email = $scope.data[0].Email;





            if ($scope.data[0].Phone != undefined) {
                $scope.Phone = $scope.data[0].Phone;
            }

            if ($scope.data[0].Fblink != '' && $scope.data[0].Fblink != "null") {
                $scope.Fblink = $scope.data[0].Fblink;
            }

            if ($scope.data[0].Instalink != '' && $scope.data[0].Instalink != "null") {
                $scope.InstaLink = $scope.data[0].Instalink;
            }

            if ($scope.data[0].Twitterlink != '' && $scope.data[0].Twitterlink != "null") {
                $scope.TwitterLink = $scope.data[0].Twitterlink;
            }

            $scope.Id = $scope.data[0].StudentId;
            $scope.State = $scope.data[0].State;
            $scope.City = $scope.data[0].City;
            $scope.Address = $scope.data[0].Address;
            $scope.LandMark = $scope.data[0].Landmark;
            $scope.PinCode = $scope.data[0].Pincode;
            $scope.Mobile = $scope.data[0].Mobile;

            if ($scope.data[0].TutId == null || $scope.data[0].TutId == '') {
                $scope.arrStdIdUserType = [];
                $scope.arrStdIdUserType.push({ Id: $scope.data[0].StudentId, UserType: "All-Students-Course" });
                sessionStorage.setItem("StudentIdUserNameForCommu", JSON.stringify($scope.arrStdIdUserType));
            }


            $scope.arrBrandStdUserType = [];
            if ($scope.data[0].TutId != null && $scope.data[0].TutId != '') {
                $scope.TutorId = $scope.data[0].TutId;
                $scope.arrBrandStdUserType.push({ Id: $scope.data[0].StudentId, UserType: "All-Brand-Students" });
                sessionStorage.setItem("BrandStudentIdUserType", JSON.stringify($scope.arrBrandStdUserType));
            }





            //$scope.Email = $scope.data[0].Email;
            //  $scope.Course = $scope.data[0].CourseId;
            //  $scope.strDuration = $scope.data[0].Duration;

            $scope.ImgOnlyOnEdit = true;
        })
    }


    // callinit for studentprofile

    $scope.callInit = function () {

        $scope.clrSession = function () {
            sessionStorage.clear();
            location.href = "/User/StudentTutorRegister";
        }

        $scope.Payment = "Cash";
        $scope.editmulticoursetable = true;

        //Calculation for due amount
        $scope.forDueAmountCalculation = function () {
            //$scope.DueAmount = $scope.NetAmount - $scope.PaidAmount;
            $scope.DueAmount = $scope.Total - $scope.PaidAmount;
        }

        $scope.validateNextInstallmentDate = function () {
        }

        $http.get('/User/GetCourses').then(function (response) {
            $scope.CourseData = response.data;
        })

        if (sessionStorage.EditData !== undefined) {
            $scope.data2 = JSON.parse(sessionStorage.EditData);
            $scope.Id = $scope.data2;
            $scope.showsendOtp = false;
            $scope.tempregno = true;
            $scope.hidebasic = false;
            $scope.coursedetails = false;
            $scope.hidecourseValidation = true;
            $scope.disableState = true;
            $scope.disableCity = true;
            $scope.disablePinCode = true;
            $scope.disableAddress = true;
            $scope.disableLandMark = true;
            $scope.disableCourse = true;
            $scope.disableDuration = true;
            $scope.disableDiscount = true;
            $scope.disableFeeAfterDiscount = true;
            $scope.disableNetAmountToPay = true;
            $scope.disableMonth = true;
            $scope.disableJoiningDate = true;
            $scope.disablePayment = true;
            $scope.disableRemarks = true;
            $scope.updateStudent = true;
            $scope.insertStudent = false;
            $scope.paymentdetails = false;
            $scope.Payment = "Cash";
            $scope.disabledJoinDate = true;
            $scope.disabledDOB = true;
            $scope.addmulticoursetable = true;
            $scope.hideAddbtn = true;
            $scope.editmulticoursetable = false;
            $scope.hideCourseDur = true;
            $scope.hideFees = true;//false => Visible
            //$scope.hideDiscount = true;
            $scope.hideFeesAfterDiscount = true; //false => Visible
            //  $scope.hideNetAmount = true;
            $scope.hideMonth = true;
            $scope.hideJoiningDate = true;
            // $scope.hideAddbtn = true;

            $http.get('/User/GetCourses').then(function (response) {
                $scope.CourseData = response.data;
            });

            $scope.Total = null;
            $http.get(`/User/EditStudents?StudentId=${$scope.Id}`).then(function (response) {
                $scope.data = response.data;
                if ($scope.data != undefined) {
                    $scope.Name = $scope.data.StudentName;
                    $scope.Phone = $scope.data.Mobile;
                    $scope.Email = $scope.data.Email;
                    $scope.State = $scope.data.State;
                    $scope.City = $scope.data.City;
                    $scope.PinCode = $scope.data.PinCode;
                    $scope.Address = $scope.data.Address;
                    $scope.LandMark = $scope.data.LandMark;
                    $scope.Course = $scope.data.Course;
                    $scope.strDuration = $scope.data.Duration;
                    $scope.mntDOB = $scope.data.DOB;
                    $scope.mntCourseValidFrom = $scope.data.ValidFrom;
                    $scope.mntCourseValidTo = $scope.data.ValidTo;
                    $scope.Fees = $scope.data.Fees;

                    //if ($scope.data.DiscountPercent != undefined || 0 || "") {
                    //    $scope.showDiscount = true;
                    //    $scope.showFeeAfterDiscount = true;
                    //    $scope.Discount = $scope.data.DiscountPercent;
                    //    $scope.FeesAfterDiscount = $scope.data.FeeAfterDiscount;
                    //} else {
                    //    $scope.showDiscount = false;
                    //    $scope.showFeeAfterDiscount = false;
                    //}

                    $scope.Discount = $scope.data.DiscountPercent;
                    $scope.FeesAfterDiscount = $scope.data.FeeAfterDiscount;

                    $scope.NetAmount = $scope.data.NetAmount;
                    $scope.Month = $scope.data.Month;
                    $scope.JoiningDate = $scope.data.JoiningDate;
                    $scope.PaymentMode = $scope.data.PaymentMode;
                    $scope.Remarks = $scope.data.Remarks;
                    $scope.TemporaryRegNo = $scope.data.TemporaryRegNo;
                    $scope.FinalRegNo = $scope.data.FinalRegNo;
                    $scope.PaymentStatus = $scope.data.PaymentStatus;
                    $scope.PaidAmount = $scope.data.PaidAmount;
                    $scope.DueAmount = $scope.data.Due;
                    $scope.dtNextInstallmentDate = $scope.data.NextInstallmentDate;
                    $scope.RemarksPayment = $scope.data.RemarksPayment;
                    $scope.hidecourseValidation = false;
                }
            });

            $http.get(`/User/EditMultipleCourseDetail?StudentId=${$scope.Id}`).then(function (response) {
                $scope.multipleCourse = response.data;
                var i = 0;
                while (i < $scope.multipleCourse.length) {
                    $scope.Total += $scope.multipleCourse[i].NetAmount;
                    $scope.Total = Math.floor($scope.Total);
                    i++;
                }
            })

        }
        else {
            $scope.insertStudent = false;
            // $scope.insertStudent = true;
            $scope.updateStudent = false;
            $scope.showsendOtp = true;
            $scope.hidebasic = true;
            $scope.coursedetails = true;
            $scope.paymentdetails = true;
            $scope.hideFees = false;
            $scope.hideFeesAfterDiscount = false;
            $scope.tempregno = false;
            $scope.hidecourseValidation = true;

            $http.get('/User/GetCourses').then(function (response) {
                $scope.CourseData = response.data;
            })

            $scope.Payment = "Cash";
        }

        $scope.emailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    }


    $scope.changeImg = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;

        var files = $scope.fullfile;
        var title = files.$ngfName;
        var reader = new FileReader();
        if (files) {
            reader.readAsDataURL(files);
            var name = files.name;
        }

        reader.addEventListener("load", function () {
            // $scope.Preview = reader.result;
            $scope.Preview = reader.result;
            $scope.ImgOnlyOnEdit = false;
            $scope.PreviewImage = true;

        }, false);

        if ($scope.Preview !== undefined) {
            $scope.Preview = name;
            $scope.Title = $scope.Preview;
        }
    }

    // Update Student
    $scope.sWc = [];
    $scope.UpdateStudentProfile = function () {
        $scope.TotalMoney = $scope.TotalMoney;
        $scope.Name = $scope.Name;
        $scope.Username = $scope.Username;
        $scope.Password = $scope.Password;

        $scope.Email = $scope.Email;
        $scope.Title = $scope.ProfileImage;
        $scope.DOBDD = moment($scope.DOB, "dd/MM/yyyy");
        $scope.DOBDD = $scope.DOBDD.toDate();
        // $scope.DOB = $scope.DOB.toDate();

        $scope.Remarks = $scope.RemarksOfCourse;
        $scope.Mobile = $scope.Phone;
        $scope.Fblink = $scope.Fblink;
        $scope.InstaLink = $scope.InstaLink;
        $scope.TwitterLink = $scope.TwitterLink;
        $scope.Id = $scope.Id;
        $scope.sWc.push({ StructureId: $scope.StructureId });
        $scope.Title = $scope.Title;
        $scope.StructureId = $scope.StructureId;

        /*========New Columns Data for Contact Details on 13 Aug 202*/
        $scope.StdContact = {};
        $scope.StdContact.State = $scope.State;
        $scope.StdContact.City = $scope.City;
        $scope.StdContact.Address = $scope.Address;
        $scope.StdContact.LandMark = $scope.LandMark;
        $scope.StdContact.PinCode = $scope.PinCode;
        $scope.StdContact.PaymentMode = $scope.Payment;

        //StructureId: $scope.StructureId
        Upload.upload({
            url: "/User/UpdateStudentProfile",
            data: {
                Total: $scope.TotalMoney,
                Id: $scope.Id, Name: $scope.Name, DOB: $scope.DOBDD, Remarks: $scope.Remarks, Mobile: $scope.Mobile, Username: $scope.Username, Password: $scope.Password,
                Email: $scope.Email, Fblink: $scope.Fblink, Instalink: $scope.InstaLink, Twitterlink: $scope.TwitterLink,
                postedfile: $scope.fullfile,
                objContact: $scope.StdContact,
                multicourse: $scope.arraydata,
                sWc: $scope.sWc
            }
        }).then(function (response) {
            $scope.data = response.data;
            $scope.showLoader = false;
            if (response.data == "Updated") {
                $scope.AddActivity($scope.Id, "Student", "Update", "Student Update Msg", "Student Update Desc");
                alert("Data Updated Successfully");
                location.href = "/User/StudentsProfile";
            }
            else if (response.data == "DateBirth") {
                swal({
                    title: 'Error!',
                    text: 'Age can not be less than 18',
                    icon: "warning",
                    dangerMode: "true"
                });
            }
            else if (response.data == "Error") {
                swal({
                    title: 'Error!',
                    text: 'Can not Update Records! Please Contact Admin',
                    icon: "warning",
                    dangerMode: "true"
                });
            }
        })
    }

    // Internship add init
    $scope.addinernshipinit = function () {
        $scope.GetAddedInternships();
    }

    //This method is also use at Adding Communication messge to Intern Student by Dilshad A. on 21 Sept 2020
    $scope.GetAddedInternships = function () {
        $http.get('/Internship/GetAddedInternships').then(function (response) {
            //$scope.data = response.data; Commented by and modified from $scope.data to $scope.GetInternshipDataForCommu  Dilshad A. 28 Sept 2020
            $scope.GetInternshipDataForCommu = response.data; //Added by Dilshad A. 28 Sept 2020 
            $scope.ShowTutors = new NgTableParams({}, { dataset: $scope.GetInternshipDataForCommu });
        });
    }


    $scope.editInternship = function (Id) {

        $scope.Id = Id;
        sessionStorage.InternshipId = JSON.stringify($scope.Id);

        $http.get('/Internship/EditInternship?Id=' + $scope.Id).then(function (response) {
            $scope.data = response.data;
            $scope.strinternship = $scope.data;
        })
    }


    $scope.deleteInternship = function (Id) {
        $scope.Id = Id;
    }


    // Delete Course 

    $scope.DeleteInternship = function (Id) {
        $http.post('/Internship/DeleteInternship', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Data Deleted Successfully");
                window.location.href = "/Internship/AddInternship";
            }
            else if (response.data == "Internshiptaken") {
                alert("Can not delete internship due to dependency");
                $("#myModal").modal('hide');
            }
            else if (response.data == "str") {
                alert("Can not delete internship due to depedency");
                $("#myModal").modal('hide');
            }
        })
    }

    // Function for Create Internship

    $scope.saveInternship = function () {

        if (sessionStorage.InternshipId !== undefined && $scope.strinternship !== undefined) {
            $scope.Id = JSON.parse(sessionStorage.InternshipId);
            $http.post('/Internship/UpdateInternship', { intId: $scope.Id, strinternship: $scope.strinternship }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    sessionStorage.clear();
                    alert("Internship Updated Successfully");
                    window.location.href = "/Internship/AddInternship";
                }
                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("Internship already exists");
                }
                else {
                    alert("Error! Problem while creating Course");
                }
            })
        }
        else {
            $scope.Internship = {};
            $scope.Internship.InternshipName = $scope.strinternship;

            $http.post('/Internship/CreateInternship', { objInternship: $scope.Internship }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    alert("Internship created successfully");
                    window.location.href = "/Internship/AddInternship";
                }
                else if (response.data == false) {
                    alert("Internship already exists");
                }
                else if (response.data == "empty") {
                    swal({
                        title: 'Error',
                        text: 'Internship is mandatory',
                        icon: "warning",
                        dangerMode: "true"
                    });
                    // alert("Please enter Course Course is mandatory");
                }

                else {
                    alert("Error! Problem while creating Course");
                }
            })
        }
    }


    // Internship Structure

    $scope.internshipstructureInit = function () {
        $http.get('/Internship/GetInternship').then(function (response) {
            $scope.InternshipData = response.data;
        })

        $http.get('/Course/GetDuration').then(function (response) {
            $scope.DurationsData = response.data;
        })

        $http.get('/Internship/GetInternshipActivity').then(function (response) {
            $scope.InternshipActivityTypeData = response.data;
        })

        $scope.showActivitytypeInsert = true;
        $scope.hideActivityType = false;
        $scope.showStipened = true;
        $scope.hidefeescolumns = true;
        $scope.UpdateBtn = false;
        $scope.InsertBtn = true;

        //$http.get('/Course/ShowCourses').then(function (response) {
        //    $scope.data = response.data;
        //    $scope.CourseDetail = new NgTableParams({}, { dataset: $scope.data });

        //})

    }


    $scope.getinternshiptype = function () {
        if ($scope.InternshipType === "Stipened") {
            $scope.showStipened = false;
            $scope.hidefeescolumns = true;
        }
        else if ($scope.InternshipType === "Free") {
            $scope.hidefeescolumns = true;
            $scope.showStipened = true;
        }
        else if ($scope.InternshipType === "Paid") {
            $scope.hidefeescolumns = false;
            $scope.showStipened = true;
        }
    }


    $scope.getInternshipMonth = function () {
        if ($scope.Duration == "1 Month") {
            $scope.Month = 1;
        }
        else if ($scope.Duration == "2 Months") {
            $scope.Month = 2;
        }
        else if ($scope.Duration == "3 Months") {
            $scope.Month = 3;
        }
    }


    $scope.getInternshipNetAmount = function () {
        if ($scope.Discount == undefined && $scope.Fees != undefined) {
            $scope.NetAmount = $scope.Fees;
        }
        else if ($scope.Discount !== undefined && $scope.Fees !== undefined) {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
        }
    }


    $scope.netamountInternshipwithdiscount = function () {
        if ($scope.Discount === undefined && $scope.Discount == 0 && $scope.Discount === "" &&
            $scope.Fees !== undefined) {
            $scope.NetAmount = $scope.Fees;
            $scope.FeeAfterDiscount = $scope.NetAmount;
        }
        else {
            $scope.NetAmount = $scope.Fees - $scope.Fees * $scope.Discount / 100;
            $scope.FeeAfterDiscount = $scope.NetAmount;
        }
    }


    // Create Course Structure Valid From Validation
    $scope.validateInternshipFromDate = function () {
        if ($scope.ValidFrom !== undefined) {
            $scope.ValidFromDate = moment($scope.ValidFrom).format("YYYY-MM-DD");
            sessionStorage.validfromdate = JSON.stringify($scope.ValidFromDate);
        }
    }

    $scope.validateInternshipToDate = function () {
        if (sessionStorage.validfromdate != null && $scope.ValidTo !== undefined) {
            $scope.From = JSON.parse(sessionStorage.validfromdate);
            $scope.To = moment($scope.ValidTo).format("YYYY-MM-DD");
            if ($scope.From >= $scope.To) {
                //   alert("End Date must be greater than start date");
                swal({
                    title: 'Error!',
                    text: 'Valid To must be greater than Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                $scope.internshipdetails.$invalid = true;
            }
            else {
                $scope.internshipdetails.$invalid = false;
            }
        }
    }


    // saveCourseDetails
    $scope.saveInternshipDetails = function () {
        $scope.CreateInternshipStructure = {};
        $scope.CreateInternshipStructure.InternshipId = $scope.Internship;
    //    $scope.CreateInternshipStructure.DurationName = $scope.strDurations.DurationName;
        $scope.CreateInternshipStructure.DurationMonths = $scope.Month;
        $scope.CreateInternshipStructure.Fees = $scope.Fees;
        $scope.CreateInternshipStructure.InternshipType = $scope.InternshipType;
        $scope.CreateInternshipStructure.Discount = $scope.Discount;
        $scope.CreateInternshipStructure.FeeAfterDiscount = $scope.FeeAfterDiscount;
        $scope.CreateInternshipStructure.TotalAmount = $scope.NetAmount;
        $scope.CreateInternshipStructure.ValidFrom = moment($scope.ValidFrom).format("YYYY-MM-DD");
        $scope.CreateInternshipStructure.ValidTo = moment($scope.ValidTo).format("YYYY-MM-DD");
        $scope.CreateInternshipStructure.LastApplyDate = moment($scope.LastApplyDate).format("YYYY-MM-DD");
        $scope.CreateInternshipStructure.TotalAvailableSeat = $scope.TotalAvailableSeat;
        $scope.CreateInternshipStructure.StipenedMoney = $scope.StipenedMoney;
        $scope.CreateInternshipStructure.DurationId = $scope.strDurations;
        $scope.CreateInternshipStructure.IsPublished = $scope.mdlInternStrIsPublished;
        $http.post('/Internship/AddInternshipStructure', {
            internshipStructure: $scope.CreateInternshipStructure,
            lstBullet: $scope.arrInterBullet, lstWhoCanApply: $scope.arrWhoCanApply, lstSkills: $scope.arrInterSkill, lstPerk: $scope.arrPerks, lstInternActivity: $scope.arrActivity
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                alert("Internship structure created successfully");
                location.href = "/Internship/GetInternshipStructure";
            }
            else if (response.data == false) {
                alert("Internship Structure with this Duration already exists");
            }
            else if (response.data == "Error") {
                alert("Error can't insert Internship Structure");
            }
        })
    }

    // Internship Structure Show
    $scope.GetInternshipData = function () {
        sessionStorage.removeItem("InternshiptById");
        $http.get('/Internship/GetInternshipData').then(function (response) {
            $scope.data = response.data;
            $scope.InternshipTable = new NgTableParams({}, { dataset: $scope.data });
        })
        $scope.hideDurId = false;
    }

    // EDit Internship Data Commented on 29 Aug By Dilshad A.
    ////$scope.EditIntership = function (intershipSingleData) {
    ////    if (intershipSingleData != '' && intershipSingleData != undefined) {
    ////        sessionStorage.setItem("InternshiptById", JSON.stringify(intershipSingleData));
    ////        window.location.href = "/Internship/InternshipStructure/";
    ////    }
    ////};

    // Get Internship Stucture Data By Id
    $scope.GetInternashipDataById = function () {

        $http.get('/Course/GetDuration').then(function (response) {
            $scope.DurationsData = response.data;
        })


        var d = sessionStorage.getItem("InternshiptById");
        d = JSON.parse(d);

        if (d != "" && d != undefined) {
            $scope.hideActivityType = true;
            $scope.showActivitytypeInsert = false;

            $scope.UpdateBtn = true;
            $scope.InsertBtn = false;



            $scope.InternStructure = d.InternStructureId;
            $scope.Internship = d.InternshipId;
            $scope.mntValidFrom = d.ValidFrom;
            $scope.mntValidTo = d.ValidTo;
            $scope.mntLastApply = d.LastApplyDate;
          //  $scope.strDurations = d.DurationId;

           // $scope.strDurationInternshipStructure.DurationId = d.DurationId.ToString();

            if ($scope.TotalAvailableSeat != undefined) {
                $scope.TotalAvailableSeat = d.TotalAvailableSeat;
            }

            $scope.Month = d.DurationMonths;
            //   $scope.Duration = d.DurationName;


            $scope.strDurations = d.DurationId;
            $scope.DurationName = d.Durations;

         //    $scope.Duration = d.DurationId;
            //$scope.arrDurationOnEdit = [];
            //if (d.Durations != "" && d.Durations != undefined && d.DurationId != "" && d.DurationId != undefined) {
            //    //d.DurationId
            //    $scope.arrDurationOnEdit.push({ DurationId: $scope.Duration, DurationName: d.Durations });
            //}

            //if ($scope.arrDurationOnEdit.length > 0) {
            //    $scope.strDurations.DurationName = JSON.stringify($scope.arrDurationOnEdit[0].DurationName);
            //    console.log(JSON.stringify($scope.strDurations.DurationName ));
            //}

            $scope.InternshipType = d.InternshipType;
            $scope.Fees = d.Fees;
            $scope.Discount = d.Discount;
            $scope.NetAmount = d.TotalAmount;
            $scope.StipenedMoney = d.Stipened;

            $scope.FeeAfterDiscount = d.FeeAfterDiscount;



            if (d.InternshipType === "Stipened") {
                $scope.showStipened = false;
                $scope.hidefeescolumns = true;
            }
            else if (d.InternshipType === "Free") {
                $scope.hidefeescolumns = true;
                $scope.showStipened = true;
            }
            else if (d.InternshipType === "Paid") {
                $scope.hidefeescolumns = false;
                $scope.showStipened = true;
            }
        }
    };

    //Edit Intership Structure  by Dilshad A. on 29 Aug 2020
    $scope.EditIntershipStructure = function (Id) {
        $http.post("/Internship/EditIntershipStructure", { intInterStructId: Id }).then(function (resInterStru) {
            sessionStorage.setItem("InterStrucById", resInterStru.data);
            window.location.href = "/Internship/InternshipStructure";
        });
    };

    // Get Duration Name for Internship Structure Add / Update

    //$scope.getDurationNameForIS = function (id) {

    //    $scope.Id = id;
    //    $http.get('/User/InternshipStructureDurationBasedOnId?Id=' + $scope.Id).then(function (response) {
    //        $scope.DurationNames = response.data;
    //    });
    //}


    $scope.getInternshipDuration = function (id) {
        $scope.Id = id;
        $http.get('/User/InternshipStructureDurationBasedOnId?Id=' + $scope.Id).then(function (response) {
            $scope.DurationNames = response.data;
        });
    }


    $scope.getInternshipName = function (id) {
        $scope.InternshipId = id;
        $http.get('/User/InternshipStructureInternshipBasedOnId?Id=' + $scope.InternshipId).then(function (response) {
            $scope.InternshipNames = response.data;
        });
    }

    // Get Intership Structure By Id. by Dilshad A. on 29 Aug 2020 
    $scope.getInternStructureById = function () {
        var allData = sessionStorage.getItem("InterStrucById");
        if (allData !== null && allData !== undefined) {
            var parseData = JSON.parse(allData);
            $scope.arrInterBullet = [];
            $scope.arrInterSkill = [];
            $scope.arrWhoCanApply = [];
            $scope.arrPerks = [];
            $scope.arrActivity = [];

            if (parseData.bullets !== undefined && parseData.bulllets != '') {
                $scope.arrInterBullet = parseData.bullets;
            }

            $scope.arrInterSkill = parseData.skillSet;
            $scope.arrWhoCanApply = parseData.wca;
            $scope.arrPerks = parseData.perks;
            $scope.arrActivity = parseData.activity;

            var d = parseData.structure[0];// Single Structure Data       

            if (d != "" && d != undefined) {
                $scope.UpdateBtn = true;
                $scope.InsertBtn = false;

                //$http.get('/Course/GetDuration').then(function (response) {
                //    $scope.DurationsData = response.data;
                //})

                $scope.InternStructure = d.InternStructureId;
                $scope.Internship = d.InternshipId;
                $scope.mntValidFrom = d.ValidFrom;
                $scope.mntValidTo = d.ValideTo;
                $scope.mntLastApply = d.LastApplyDate;
                $scope.mdlInternStrIsPublished = d.IsPublished;

                if (d.TotalAvailableSeat !== undefined) {
                    $scope.TotalAvailableSeat = d.TotalAvailableSeat;
                }

                $scope.Month = d.DurationMonths;
                //  $scope.Duration = d.DurationName;
                $scope.InternshipType = d.InternshipType;
                $scope.Fees = d.Fees;
                $scope.Discount = d.Discount;
                $scope.NetAmount = d.TotalAmount;

              //  $scope.Duration = d.DurationId; /*.toString()*/

                // Commented by Atul On 10 Sept.
                //  $scope.StipenedMoney = d.Stipened;

                $scope.StipenedMoney = d.StipenedMoney;
                $scope.FeeAfterDiscount = d.FeeAfterDiscount;

                $scope.InternshipNameOnEdit = d.InternshipName;

                $scope.strDurations = d.DurationId;
                $scope.DurationName = d.Durations;

                //$scope.arrDurationOnEdit = [];
                //if (d.Durations != "" && d.Durations != undefined && d.DurationId != "" && d.DurationId != undefined) {
                    
                //    $scope.arrDurationOnEdit.push({ DurationId: $scope.Duration, DurationName: d.Durations });
                //}

                //if ($scope.arrDurationOnEdit.length > 0) {
                //    $scope.strDurations = JSON.stringify($scope.arrDurationOnEdit[0].DurationName);
                //    console.log($scope.strDurations);
                //}

                

                if (d.InternshipType === "Stipened") {
                    $scope.showStipened = false;
                    $scope.hidefeescolumns = true;

                    $scope.hideFeeDiscount = true;
                    $scope.hideFeeNetAmount = true;


                }
                else if (d.InternshipType === "Free") {
                    $scope.hidefeescolumns = true;
                    $scope.showStipened = true;
                }
                else if (d.InternshipType === "Paid") {
                    $scope.hidefeescolumns = false;
                    $scope.showStipened = true;
                }
            }
        }
    }

    // Update Internship Structure:
    $scope.updateInternshipDetails = function () {
        $scope.tblis = {};
        var d = sessionStorage.getItem("InternshiptById");
        d = JSON.parse(d);

        if (d != '' && d != undefined) {
            $scope.tblis.InternStructureId = d.InternStructureId;
        }

        $scope.tblis.IsPublished = $scope.mdlInternStrIsPublished;
        $scope.tblis.InternshipId = $scope.Internship;

    //    $scope.tblis.DurationName = $scope.strDurations.DurationName;
        $scope.tblis.DurationMonths = $scope.Month;
        $scope.tblis.Fees = $scope.Fees;
        $scope.tblis.InternshipType = $scope.InternshipType;
        $scope.tblis.Discount = $scope.Discount;
        $scope.tblis.FeeAfterDiscount = $scope.FeeAfterDiscount;
        $scope.tblis.TotalAmount = $scope.NetAmount;
        $scope.tblis.ValidFrom = moment($scope.ValidFrom).format("YYYY-MM-DD");
        $scope.tblis.ValidTo = moment($scope.ValidTo).format("YYYY-MM-DD");
        $scope.tblis.LastApplyDate = moment($scope.LastApplyDate).format("YYYY-MM-DD");
        $scope.tblis.TotalAvailableSeat = $scope.TotalAvailableSeat;
        $scope.tblis.StipenedMoney = $scope.StipenedMoney;
        $scope.tblis.DurationId = $scope.strDurations;

        $http.post('/Internship/UpdateInternshipStructure', {
            tblis: $scope.tblis,
            InternStructureId: $scope.InternStructure,
            lstBullet: $scope.arrInterBullet, deleteBulletArray: $scope.deleteBulletArray, lstWhoCanApply: $scope.arrWhoCanApply, deleteWhoApply: $scope.deleteWhoApply, lstSkills: $scope.arrInterSkill, deleteSkillArray: $scope.deleteSkillArray, lstPerk: $scope.arrPerks, deletePerkArray: $scope.deletePerkArray, lstInternActivity: $scope.arrActivity, deleteActivityArray: $scope.deleteActivityArray
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                sessionStorage.clear();
                alert("Internship Structure Details Updated Successfully");
                window.location.href = "/Internship/GetInternshipStructure";
            }
            else if (response.data == "Duration") {
                // sessionStorage.clear();
                alert("Can not update Duration");
                // alert("Course with this Duration already exists can not update");
            }
            else if (response.data == false) {
                //  sessionStorage.clear();
                alert("Problem while updating internship details");
            }
            else if (response.data == "ValidTo") {
                swal({
                    title: 'Error!',
                    text: 'ValidTo must be greater than or equal to Valid From',
                    icon: "warning",
                    dangerMode: "true"
                });
                //alert("ValidTo must be greater than Valid From");
                $scope.coursedetails.$invalid = true;
            }
        });
    }

    // Get Internship List 
    $scope.InternshipListDetail = function () {
        $http.get("/User/InternshipListDetail").then(function (resIntrnList) {
            $scope.InternList = resIntrnList.data;
        });
    }


    $scope.getInternshipDetails = function (Id) {

        $scope.Id = Id;
        sessionStorage.InternshipId = JSON.stringify($scope.Id);
        location.href = "/User/InternshipListDetails";
    }

    // Internship List Details public view getting data

    $scope.getInternshipStructureBasedOnId = function () {
        if (sessionStorage.InternshipId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.InternshipId);
        }

        $http.get('/User/InternshipStructureDataBasedOnId?Id=' + $scope.Id).then(function (response) {
            $scope.InternshipBasedOnId = response.data;
            $scope.data = JSON.parse($scope.InternshipBasedOnId);

            if ($scope.data["structure"] != '' && $scope.data["structure"] != undefined) {

                $scope.InternshipName = $scope.data["structure"][0].InternshipName;
                $scope.InternshipType = $scope.data["structure"][0].InternshipType;
                $scope.LastApplyDate = $scope.data["structure"][0].LastApplyDate;
                $scope.ValidFrom = $scope.data["structure"][0].ValidFrom;
                $scope.DurationMonths = $scope.data["structure"][0].DurationMonths;
                $scope.DurationName = $scope.data["structure"][0].DurationName;
                $scope.TotalSeat = $scope.data["structure"][0].TotalAvailableSeat;

                //      $scope.Total =    $scope.data["structure"][0].TotalAmount;
                //       $scope.LastApply = $scope.data["structure"][0].LastApplyDate;
                //       $scope.TotalAvailableSeat =     $scope.data["structure"][0].TotalAvailableSeat;
            }


            // Internship Details

            if ($scope.data["Heading"] != '' && $scope.data["Heading"] != undefined) {
                $scope.CourseContentHeading = $scope.data["Heading"][0].CourseContentHeading;
                $scope.SubHeading = $scope.data["Heading"][0].SubHeading;
                $scope.ShortDescription = $scope.data["Heading"][0].ShortDescription;
                $scope.DateCreated = $scope.data["Heading"][0].DateCreated;
            }

            // Storing description data in array

            $scope.descriptionarray = [];
            $scope.data7 = [];

            if ($scope.data["description"] != '' && $scope.data["description"] != undefined) {
                for (var i = 0; i < $scope.data["description"].length; i++) {
                    //  $scope.data7 = $scope.data["description"][i];
                    $scope.descriptionarray.push($scope.data["description"][i]);
                }

            }


            // Storing Prerequisities for course

            $scope.prerequisitearray = [];
            $scope.data8 = [];

            if ($scope.data["prerequisities"] != '' && $scope.data["prerequisities"] != undefined) {
                for (var i = 0; i < $scope.data["prerequisities"].length; i++) {
                    //  $scope.data8 = $scope.data["prerequisities"][i];
                    $scope.prerequisitearray.push($scope.data["prerequisities"][i]);
                }


            }


            // Storing bullets data in array

            $scope.bulletsarray = [];
            $scope.data2 = [];
            if ($scope.data["bullets"] != '' && $scope.data["bullets"] != undefined && $scope.data["bullets"].length > 0) {

                for (var i = 0; i < $scope.data["bullets"].length; i++) {
                    $scope.data2 = $scope.data["bullets"][i];
                }

                $scope.bulletsarray.push($scope.data2);
            }


            // Storing skills data in array

            $scope.skillSetarray = [];
            $scope.data3 = [];
            if ($scope.data["skillSet"] != '' && $scope.data["skillSet"] != undefined && $scope.data["skillSet"].length > 0) {

                for (var i = 0; i < $scope.data["skillSet"].length; i++) {
                    $scope.data3 = $scope.data["skillSet"][i];
                }

                $scope.skillSetarray.push($scope.data3);
            }


            // Intern Apply Point

            $scope.wcaarray = [];
            $scope.data4 = [];
            if ($scope.data["wca"] != '' && $scope.data["wca"] != undefined && $scope.data["wca"].length > 0) {

                for (var i = 0; i < $scope.data["wca"].length; i++) {
                    $scope.data4 = $scope.data["wca"][i];
                }

                $scope.wcaarray.push($scope.data4);
            }


            // Perk (Certificate etc) stored in array

            $scope.perksaarray = [];
            $scope.data5 = [];
            if ($scope.data["perks"] != '' && $scope.data["perks"] != undefined && $scope.data["perks"].length > 0) {

                for (var i = 0; i < $scope.data["perks"].length > 0; i++) {
                    $scope.data5 = $scope.data["perks"][i];
                }

                $scope.perksaarray.push($scope.data5);
            }


            // Activity array

            $scope.activityaarray = [];
            $scope.data6 = [];
            if ($scope.data["activity"] != '' && $scope.data["activity"] != undefined && $scope.data["activity"].length > 0) {

                for (var i = 0; i < $scope.data["activity"].length > 0; i++) {
                    $scope.data6 = $scope.data["activity"][i];
                }

                $scope.activityaarray.push($scope.data6);
            }
            //$scope.strCourse = $scope.data;
        })



    }


    // Get Course List 
    $scope.PublicCoursesListDetails = function () {
        $http.get("/User/PublicCoursesListDetails").then(function (resCourseList) {
            $scope.CourseListData = resCourseList.data;
        });
    }

    // Get Course List Details

    $scope.getCoursesDetails = function (Id) {
        $scope.Id = Id;
        sessionStorage.CourseStructureId = JSON.stringify($scope.Id);
        location.href = "/User/CoursesListDetails";
    }

    //$scope.getCourseStructureDetailsBasedOnId = function () {

    //    if (sessionStorage.CourseStructureId != undefined) {
    //        $scope.Id = JSON.parse(sessionStorage.CourseStructureId);
    //    }

    //    $http.get('/User/CourseStructureDataBasedOnId?Id=' + $scope.Id).then(function (response) {
    //        $scope.CourseBasedOnId = response.data;
    //        $scope.data = JSON.parse($scope.CourseBasedOnId);
    //        if ($scope.data["struc"] != '' && $scope.data["struc"] != undefined) {
    //            $scope.CourseName = $scope.data["struc"][0].CourseName;
    //            $scope.Fees = $scope.data["struc"][0].Fees;
    //            $scope.NetAmount = $scope.data["struc"][0].NetAmount;
    //            $scope.CategoryName = $scope.data["struc"][0].CategoryName;
    //            $scope.DurationName = $scope.data["struc"][0].DurationName;
    //            $scope.ValidFrom = $scope.data["struc"][0].ValidFrom;
    //            $scope.ValidTo = $scope.data["struc"][0].ValidTo;
    //        }


    //        // Course Content Details

    //        if ($scope.data["Heading"] != '' && $scope.data["Heading"] != undefined) {
    //            $scope.CourseContentHeading = $scope.data["Heading"][0].CourseContentHeading;
    //            $scope.SubHeading = $scope.data["Heading"][0].SubHeading;
    //            $scope.ShortDescription = $scope.data["Heading"][0].ShortDescription;
    //            $scope.DateCreated = $scope.data["Heading"][0].DateCreated;
    //        }

    //        // Storing description data in array

    //        $scope.descriptionarray = [];
    //        $scope.data7 = [];

    //        if ($scope.data["description"] != '' && $scope.data["description"] != undefined) {
    //            for (var i = 0; i < $scope.data["description"].length; i++) {
    //                //  $scope.data7 = $scope.data["description"][i];
    //                $scope.descriptionarray.push($scope.data["description"][i]);
    //            }

    //        }


    //        // Storing Prerequisities for course

    //        $scope.prerequisitearray = [];
    //        $scope.data8 = [];

    //        if ($scope.data["prerequisities"] != '' && $scope.data["prerequisities"] != undefined) {
    //            for (var i = 0; i < $scope.data["prerequisities"].length; i++) {
    //                //  $scope.data8 = $scope.data["prerequisities"][i];
    //                $scope.prerequisitearray.push($scope.data["prerequisities"][i]);
    //            }
    //        }
    //    })
    //}

    $scope.getCourseStructureDetailsBasedOnId = function () {

        if (sessionStorage.CourseStructureId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.CourseStructureId);
        }

        $http.get('/User/CourseStructureDataBasedOnId?Id=' + $scope.Id).then(function (response) {
            $scope.CourseBasedOnId = response.data;

            //if ($scope.CourseBasedOnId != undefined && $scope.CourseBasedOnId != "") {
            //    $scope.data = JSON.parse($scope.CourseBasedOnId);
            //}

            $scope.data = JSON.parse($scope.CourseBasedOnId);

            // Added for new Content By Dilshad A. on 31 Oct 2020 
            $scope.DescriptionsContent = $scope.data.description;
            $scope.HeadingsContent = $scope.data.Heading;
            // END Added for new Content By Dilshad A. on 31 Oct 2020

            if ($scope.data["struc"] != '' && $scope.data["struc"] != undefined) {
                $scope.CourseName = $scope.data["struc"][0].CourseName;
                $scope.Fees = $scope.data["struc"][0].Fees;
                $scope.NetAmount = $scope.data["struc"][0].NetAmount;
                $scope.CategoryName = $scope.data["struc"][0].CategoryName;
                $scope.DurationName = $scope.data["struc"][0].DurationName;
                $scope.ValidFrom = $scope.data["struc"][0].ValidFrom;
                $scope.ValidTo = $scope.data["struc"][0].ValidTo;
            }


            // Course Content Details

            if ($scope.data["Heading"] != '' && $scope.data["Heading"] != undefined) {
                $scope.CourseContentHeading = $scope.data["Heading"][0].CourseContentHeading;
                $scope.SubHeading = $scope.data["Heading"][0].SubHeading;
                $scope.ShortDescription = $scope.data["Heading"][0].ShortDescription;
                $scope.DateCreated = $scope.data["Heading"][0].DateCreated;
            }

            // Storing description data in array

            $scope.descriptionarray = [];
            $scope.data7 = [];

            if ($scope.data["description"] != '' && $scope.data["description"] != undefined) {
                for (var i = 0; i < $scope.data["description"].length; i++) {
                    //  $scope.data7 = $scope.data["description"][i];
                    $scope.descriptionarray.push($scope.data["description"][i]);
                }

            }


            // Storing Prerequisities for course

            $scope.prerequisitearray = [];
            $scope.data8 = [];

            if ($scope.data["prerequisities"] != '' && $scope.data["prerequisities"] != undefined) {
                for (var i = 0; i < $scope.data["prerequisities"].length; i++) {
                    //  $scope.data8 = $scope.data["prerequisities"][i];
                    $scope.prerequisitearray.push($scope.data["prerequisities"][i]);
                }
            }


            //x.s.DurationName,
            //    ValidFrom = x.s.ValidFrom.ToString("dd/MM/yyyy"),
            //    ValidTo = x.s.ValidTo.ToString("dd/MM/yyyy"),
            //    x.s.Fees,
            //    x.s.NetAmount,
            //    x.d.CourseId,
            //    x.d.CourseName,
            //    x.catg.CategoryName,*@

        })
    }

    // Get Tutor List
    $scope.TutorListDetail = function () {
        $http.get("/User/TutorListDetail").then(function (resTutorList) {
            $scope.TutorsListShow = resTutorList.data;

        })
    }


    // Forget Password Init
    $scope.forgetpasswordinit = function () {

    }


    // Send Password to Student when student forgets password

    $scope.verifyemailsendpass = function () {
        if ($scope.Email == undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if ($scope.emailpattern.test($scope.Email)) {
                $http.post('/User/VerifyStudentMailSendPassword', { Email: $scope.Email }).then(function (response) {
                    $scope.data = response.data;
                    if (response.data == true) {
                        alert("Password  Send to Mail Sucessfully");
                        if (window.confirm("Do you really want to login?")) {
                            location.href = "/User/StudentLogin";
                        }
                    }
                    else if (response.data == "NotRegistered") {
                        alert("You have not registered");
                    }

                })
            }
        }
    }


    // Send Password to Student when student forgets password

    $scope.verifyemailsendpasstoTutor = function () {
        if ($scope.Email == undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if ($scope.emailpattern.test($scope.Email)) {

                $http.post('/User/VerifyTutorMailSendPassword', { Email: $scope.Email }).then(function (response) {

                    $scope.data = response.data;


                    if (response.data == true) {
                        alert("Password  Send to Mail Sucessfully");
                        if (window.confirm("Do you really want to login?")) {
                            location.href = "/User/StudentLogin";
                        }
                    }
                    else if (response.data == "NotRegistered") {
                        alert("You have not registered");
                    }

                })
            }
        }
    }


    $scope.shPassword = true;

    // Get User Details


    $scope.GetUsers = function () {
        $scope.InsertBtn = true;
        $scope.showStudentsCode = false;
        $scope.showTutorsCode = false;
        // New Changes 21 Aug. Atul Sh. Hide and show tutor and student data on init and getting tutor and student code
        $scope.shStudentCode = false;
        $scope.shTutorCode = false;
        // $scope.shTutorCode = false;

        $scope.shBrandTutorCode = false;

        $scope.shBrandTutorUser = false;

        $scope.shInternCode = false;
        $scope.shInternTutorUser = false;

        $scope.showTutorsCode = false;



        $http.get('/User/BrandTutorData').then(function (response) {
            $scope.BrandTutorsData = response.data;
        })

        $http.get('/Admin/GetStudentsCode').then(function (response) {
            $scope.StudentCode = response.data;
        })

        $http.get('/Admin/GetTutorCode').then(function (response) {
            $scope.TutorCode = response.data;
        })

        $http.get('/Admin/GetBrandTutorCode').then(function (response) {
            $scope.BrandTutorCode = response.data;
        })

        $http.get('/Admin/GetInternCode').then(function (response) {
            $scope.InternCode = response.data;
        })


        $http.get('/Admin/GetInternTutorCode').then(function (response) {
            $scope.InternTutorCode = response.data;
        })

        $http.get('/Admin/InternTutorData').then(function (response) {
            $scope.InternTutorsData = response.data;
        })

        // End 21 Aug. new changes

        // Get Tutors Data (Id,Name)
        $http.get("/Admin/GetTutorsData").then(function (response) {
            $scope.TutorsData = response.data;
        })

        // Get student Details(id/Name) 
        $http.get("/Admin/BindStudents").then(function (response) {
            $scope.StudentsData = response.data;
        })

        if (sessionStorage.UsersData !== undefined && sessionStorage.UsersData != '') {

            $scope.disabletutor = true;
            $scope.InsertBtn = false;
            $scope.UpdateBtn = true;
            // $scope.mdlUserType = "Tutor";
            $scope.shPassword = false;
            $scope.disableUserType = true;



            $http.get("/Admin/BindStudents").then(function (response) {
                $scope.StudentsData = response.data;
            })

            $http.get("/Admin/GetTutorsData").then(function (response) {
                $scope.TutorsData = response.data;
            })

            $http.get('/Admin/GetBrandTutorCode').then(function (response) {
                $scope.BrandTutorCode = response.data;
            })

            $scope.EditedData = JSON.parse(sessionStorage.UsersData);

            if ($scope.EditedData != undefined && $scope.EditedData != '') {
                $scope.mdlChkIsPublishedUser = $scope.EditedData.IsPublished;
                $scope.mdlUserType = $scope.EditedData.UserType;

                if ($scope.EditedData.UserType === "Student") {
                    $scope.mdlStudentId = $scope.EditedData.Id;
                    $scope.shStudentUser = true;
                    $scope.shTutorUser = false;
                    $scope.showStudentsCode = true;
                    $scope.showTutorsCode = false;
                    $scope.StudentsCode = $scope.EditedData.CompletedId;
                }
                else if ($scope.EditedData.UserType === "Tutor") {
                    $scope.mdlUserId = $scope.EditedData.Id;
                    $scope.shStudentUser = false;
                    $scope.shTutorUser = true;
                    $scope.showTutorsCode = true;
                    $scope.showStudentsCode = false;
                    $scope.TutorsCode = $scope.EditedData.CompletedId;
                }

                else if ($scope.EditedData.UserType === "Intern") {
                    $scope.mdlInternId = $scope.EditedData.Id;
                    $scope.shStudentUser = false;
                    $scope.shTutorUser = false;
                    $scope.showInternCode = true;
                    $scope.showTutorsCode = false;
                    $scope.showStudentsCode = false;
                    $scope.shBrandTutorUser = false;
                    $scope.shInternUser = true;

                    $scope.InternsCode = $scope.EditedData.CompletedId;
                }


                else if ($scope.EditedData.UserType === "Brand-Tutor") {

                    $scope.mdlBrandTutorId = $scope.EditedData.Id;
                    $scope.shStudentUser = false;
                    $scope.shTutorUser = false;
                    $scope.shInternUser = false;
                    $scope.showInternCode = false;
                    $scope.showBrandTutorsCode = true;
                    $scope.showInternCode = false;
                    $scope.showTutorsCode = false;
                    $scope.showStudentsCode = false;
                    //    $scope.shBrandTutorUser = false;
                    $scope.shBrandTutorUser = true;
                    $scope.BrandTutorsCode = $scope.EditedData.CompletedId;
                }


                else if ($scope.EditedData.UserType === "Intern-Tutor") {

                    $scope.mdlInternTutorId = $scope.EditedData.Id;
                    $scope.shStudentUser = false;
                    $scope.shTutorUser = false;
                    $scope.shInternUser = false;
                    $scope.showInternCode = false;
                    $scope.showBrandTutorsCode = false;
                    $scope.shInternTutorCode = false;

                    $scope.showInternTutorsCode = true;
                    $scope.shInternTutorUser = true;

                    $scope.showInternCode = false;
                    $scope.showTutorsCode = false;
                    $scope.showStudentsCode = false;
                    //    $scope.shBrandTutorUser = false;
                    $scope.shBrandTutorUser = false;


                    $scope.InternTutorsCode = $scope.EditedData.CompletedId;
                }




                $scope.mdlUserId = $scope.EditedData.Id;
                $scope.UsersId = $scope.EditedData.UserId;
                $scope.mdlUserName = $scope.EditedData.UserName;
                $scope.mdlPassword = $scope.EditedData.Password;
            }
        }

        else {
            $scope.shPassword = true;
            $scope.disableUserType = false;
            $scope.disabletutor = false;
            $scope.InsertBtn = true;
            $scope.UpdateBtn = false;
            $http.get("/Admin/GetTutorsData").then(function (response) {
                $scope.TutorsData = response.data;
            })
        }
    }


    // Get User (id,name) by type Student/Tutor

    // Get User (id,name) by type Student/Tutor
    $scope.GetUserByType = function () {
        if ($scope.mdlUserType === "Tutor") {
            $scope.shStudentUser = false;
            $scope.shTutorUser = true;

            // NEw Changes On 21 Aug. for showing and hidinng student or tutor code based on User Type By Atul Sh.
            $scope.shStudentCode = false;
            $scope.shTutorCode = true;
            $scope.shStudentCode = false;
            $scope.shInternTutorCode = false;
            // End

        }
        else if ($scope.mdlUserType === "Student") {
            $scope.shStudentUser = true;
            $scope.shTutorUser = false;
            // NEw Changes On 21 Aug. for showing and hidinng student or tutor code based on User Type By Atul Sh.
            $scope.shStudentCode = true;
            $scope.shTutorCode = false;
            $scope.shStudentCode = true;

            // End
        }
        else if ($scope.mdlUserType === "Intern") {
            $scope.shInternUser = true;
            $scope.shStudentUser = false;
            $scope.shTutorUser = false;
            $scope.shStudentCode = false;
            $scope.shTutorCode = false;
            $scope.shInternCode = true;

        }
        else if ($scope.mdlUserType === "Brand-Tutor") {
            $scope.shBrandTutorUser = true;
            $scope.shInternUser = false;
            $scope.shStudentUser = false;
            $scope.shTutorUser = false;
            $scope.shStudentCode = false;
            $scope.shTutorCode = false;

            //$scope.shBrandTutorCode = true;

            $scope.shBrandTutorCode = true;

            $scope.shInternCode = false;

            $scope.showBrandTutorsCode = false;
        }
        else if ($scope.mdlUserType === "Intern-Tutor") {

            $scope.shInternTutorCode = true;
            $scope.shBrandTutorUser = false;
            $scope.shInternUser = false;
            $scope.shStudentUser = false;
            $scope.shTutorUser = false;
            $scope.shStudentCode = false;
            $scope.shTutorCode = false;
            $scope.shBrandTutorCode = false;
            $scope.shInternCode = false;
            $scope.shInternTutorUser = true;
        }

    }

    // All User Details (Tutor/Student : Credential by admin)
    $scope.getusersbyadmininit = function () {
        $scope.hidePassword = true;
        $http.get("/Admin/GetUsers").then(function (resUsers) {
            $scope.Users = resUsers.data;
            $scope.ShowAllUsers = new NgTableParams({}, { dataset: resUsers.data });
        });
    }

    // Add User for active and creadentials
    $scope.AddUser = function () {
        $scope.clsUser = {};

        $scope.clsUser.UserType = $scope.mdlUserType;

        if ($scope.mdlUserType === "Student") {
            $scope.clsUser.Id = $scope.mdlStudentId;
            $scope.clsUser.CompletedId = $scope.StudentCode;
        }
        else if ($scope.mdlUserType === "Tutor") {
            $scope.clsUser.Id = $scope.mdlUserId;
            $scope.clsUser.CompletedId = $scope.TutorCode;
        }
        else if ($scope.mdlUserType === "Intern") {
            $scope.clsUser.Id = $scope.mdlInternId;
            $scope.clsUser.CompletedId = $scope.InternCode;
            //$scope.clsUser.CompletedId = $scope.TutorCode;
        }
        else if ($scope.mdlUserType === "Intern-Tutor") {
            $scope.clsUser.Id = $scope.mdlInternTutorId;
            $scope.clsUser.CompletedId = $scope.InternTutorCode;

        }

        else if ($scope.mdlUserType === "Brand-Tutor") {
            $scope.clsUser.Id = $scope.mdlBrandTutorId;
            $scope.clsUser.CompletedId = $scope.BrandTutorCode;
        }

        $scope.clsUser.IsPublished = $scope.mdlChkIsPublishedUser;
        $scope.clsUser.UserName = $scope.mdlUserName;
        $scope.clsUser.Password = $scope.mdlPassword;

        $http.post("/Admin/AddUsers", { createuser: $scope.clsUser }).then(function (resAddUser) {
            $scope.data = resAddUser.data;
            if (resAddUser.data == true) {
                alert("User Successfully Created.");
                location.href = "/Admin/ViewUsers";
            }
            else {
                alert("Failed Something went wrong. Please try again.");
            }
        });
    }


    $scope.editUserDetail = function (user) {

        sessionStorage.UsersData = JSON.stringify(user);
        location.href = "/Admin/AddUser";
    }

    $scope.UpdateUser = function () {

        $scope.clsUser = {};
        $scope.clsUser.IsPublished = $scope.mdlChkIsPublishedUser;

        $scope.clsUser.UserType = $scope.mdlUserType;

        if ($scope.mdlUserType === "Student") {
            $scope.clsUser.Id = $scope.mdlStudentId;
            $scope.shTutorUser = false;
            $scope.shStudentUser = true;


            $scope.showStudentsCode = true;
            $scope.showTutorsCode = false;

            $scope.StudentsCode = $scope.EditedData.CompletedId;

        }
        else if ($scope.mdlUserType === "Tutor") {
            $scope.clsUser.Id = $scope.mdlUserId;
            $scope.shTutorUser = true;
            $scope.shStudentUser = false;

            $scope.showStudentsCode = false;
            $scope.showTutorsCode = true;

            $scope.TutorsCode = $scope.EditedData.CompletedId;
        }

        else if ($scope.mdlUserType === "Brand-Tutor") {
            $scope.shBrandTutorUser = true;
            $scope.showBrandTutorsCode = true;
            $scope.clsUser.Id = $scope.mdlBrandTutorId;
            $scope.shTutorUser = false;
            $scope.shStudentUser = false;
            $scope.showStudentsCode = false;
            $scope.showTutorsCode = false;
            $scope.shBrandTutorCode = false;
            $scope.BrandTutorCode = $scope.EditedData.CompletedId;
        }

        else if ($scope.mdlUserType === "Intern-Tutor") {
            $scope.shBrandTutorUser = false;
            $scope.showBrandTutorsCode = false;
            $scope.shInternTutorUser = true;
            $scope.showInternTutorsCode = true;

            $scope.shInternTutorCode = false;

            $scope.clsUser.Id = $scope.mdlInternTutorId;
            $scope.shTutorUser = false;
            $scope.shStudentUser = false;
            $scope.shInternUser = false;
            $scope.shBrandTutorUser = false;



            $scope.showStudentsCode = false;
            $scope.showTutorsCode = false;
            $scope.shBrandTutorCode = false;

            $scope.InternTutorsCode = $scope.EditedData.CompletedId;
        }

        $scope.clsUser.Id = $scope.mdlUserId;
        $scope.clsUser.UserId = $scope.UsersId;
        $scope.clsUser.UserName = $scope.mdlUserName;
        //  $scope.clsUser.Password = $scope.mdlPassword;

        $http.post("/Admin/UpdateUsers", { updateuser: $scope.clsUser }).then(function (resAddUser) {

            $scope.data = resAddUser.data;
            if (resAddUser.data == true) {
                sessionStorage.clear();
                alert("User Successfully Updated.");
                location.href = "/Admin/ViewUsers";
            }

            else {
                alert("Failed Something went wrong. Please try again.");
            }

        });
    }


    // Tutor Public Profile Scripts

    // Get Tutor by ID
    $scope.GetTutorById = function () {
        $scope.TutorData = sessionStorage.getItem("TutorProfileById");
        $scope.TutorData = JSON.parse($scope.TutorData);
    }

    // For Public Tutor Profile
    $scope.TutorPublicProfileById = function (id) {
        $http.post("/User/TutorPublicProfileById", { intTutorId: id }).then(function (resTutor) {
            sessionStorage.setItem("TutorProfileById", JSON.stringify(resTutor.data));
            //$scope.TutorById = resTutor.data;
            window.location.href = "/User/TutorProfile";
        });
    }

    // Add Multiple Intern Bullet Points By Dilshad A. on 26 2020

    $scope.arrInterBullet = [];
    $scope.AddMultipleInternBulletPoints = function () {
        if ($scope.mdlInternBulletPoint !== "" && $scope.mdlInternBulletPoint !== undefined) {
            $scope.arrInterBullet.push({ IsPublished: $scope.mdlBulletIsPublished, BulletPoint: $scope.mdlInternBulletPoint });

        }
        else {
            alert("Bullet point is mandatory.");
        }
        $scope.mdlBulletIsPublished = "";
        $scope.mdlInternBulletPoint = "";
    }


    $scope.deleteBulletArray = [];

    // Delete Bullet Points
    $scope.DeleteBulletPoints = function (index) {

        //swal({
        //    title: "Are you sure?",
        //    text: "Once deleted, you will not be able to recover this imaginary file!",
        //    icon: "warning",
        //    buttons: true,
        //    dangerMode: true,
        //})
        //    .then((willDelete) => {
        //        if (willDelete) {
        //            swal("Poof! Your imaginary file has been deleted!", {
        //                icon: "success",
        //            });
        //        } else {
        //            swal("Your imaginary file is safe!");
        //        }
        //    });


        var data2 = $scope.arrInterBullet[index];
        $scope.deleteBulletArray.push({ Index: index, BulletId: data2.BulletId });
        $scope.arrInterBullet.splice(index, 1);



    }

    //     var data = $scope.arrInterBullet[index];
    //     $scope.arrInterBullet.splice(index, 1);



    // View Course By Dlshad A. on 26 Aug 2020
    $scope.ViewCourse = function () {
        $http.get('/Course/ShowCourses').then(function (response) {
            $scope.CourseRecords = response.data;
        });
    }

    // Add Multiple Intern Skill By Dlshad A. on 26 Aug 2020
    // * Changes

    // Commented Code


    $scope.deleteBulletArray = [];

    // Delete Bullet Points
    $scope.DeleteBulletPoints = function (index) {

        var data2 = $scope.arrInterBullet[index];
        $scope.deleteBulletArray.push({ Index: index, BulletId: data2.BulletId });

        $scope.arrInterBullet.splice(index, 1);
    }


    // End Code


    $scope.arrInterSkill = [];
    $scope.AddMultipleInternSkill = function () {
        if ($scope.CourseName !== "" && $scope.CourseName !== undefined) {
            $scope.arrInterSkill.push({ IsPublished: $scope.mdlSkillIsPublished, CourseName: $scope.CourseName });
            $scope.mdlSkillIsPublished = "";
            $scope.CourseName = "";
        }
    }


    $scope.deleteSkillArray = [];

    // Delete Intern Skills By Dlshad A. on 26 Aug 2020
    $scope.DeleteInternSkills = function (index) {


        var data2 = $scope.arrInterSkill[index];
        $scope.deleteSkillArray.push({ InternSkillId: data2.InternSkillId, CourseName: data2.CourseName });
        $scope.arrInterSkill.splice(index, 1);

        //  var data = $scope.arrInterSkill[index];
        //  $scope.arrInterSkill.splice(index, 1);

    }

    // Add Multiple WhoCanApply By Dlshad A. on 26 Aug 2020
    $scope.arrWhoCanApply = [];
    $scope.AddMultipleWhoCanApply = function () {
        if ($scope.mdlWhoCanApply !== "" && $scope.mdlWhoCanApply !== undefined) {
            $scope.arrWhoCanApply.push({ IsPublished: $scope.mdlApplyPublished, ApplyPoint: $scope.mdlWhoCanApply });
            $scope.mdlWhoCanApply = "";
            $scope.mdlApplyPublished = "";
        }
        else {
            alert("Point is mandatory.");
        }
    }


    $scope.deleteWhoApply = [];

    // Delete Intern Skills By Dlshad A. on 26 Aug 2020
    $scope.DeleteWhoCanApply = function (index) {

        //     alert(index);

        var data2 = $scope.arrWhoCanApply[index];
        $scope.deleteWhoApply.push({ InternApplyId: data2.InternApplyId, ApplyPoint: data2.ApplyPoint });
        $scope.arrWhoCanApply.splice(index, 1);

        //       var data = $scope.arrWhoCanApply[index];
        //       $scope.arrWhoCanApply.splice(index, 1);

    }

    // Add Multiple Perks By Dlshad A. on 26 Aug 2020
    $scope.arrPerks = [];
    $scope.AddMultiplePerks = function () {
        if ($scope.mdlPerks !== "" && $scope.mdlPerks !== undefined) {
            $scope.arrPerks.push({ IsPublished: $scope.mdlPerkPublished, PerkName: $scope.mdlPerks });
            $scope.mdlPerks = "";
            $scope.mdlPerkPublished = "";
        }
        else {
            alert("Point is mandatory.");
        }
    }

    $scope.deletePerkArray = [];

    // Delete Intern Perks By Dlshad A. on 26 Aug 2020
    $scope.DeletePerks = function (index) {

        var data2 = $scope.arrPerks[index];
        $scope.deletePerkArray.push({ InternPerkId: data2.InternPerkId, PerkName: data2.PerkName });
        $scope.arrPerks.splice(index, 1);


        //    var data = $scope.arrPerks[index];
        //    $scope.arrPerks.splice(index, 1);

    }

    // Add Multiple Activity By Dlshad A. on 26 Aug 2020
    $scope.arrActivity = [];
    $scope.AddMultipleActivity = function () {
        if ($scope.mdlInternActivityType !== "" && $scope.mdlInternActivityType !== undefined
            && $scope.mdlActivity !== "" && $scope.mdlActivity !== undefined) {
            $scope.arrActivity.push({ IsPublished: $scope.mdlActivityPublished, ActivityPoint: $scope.mdlActivity, ActivityType: $scope.mdlInternActivityType.InternActivityTypeId, ActivityTypeId: $scope.mdlInternActivityType.InternActivityType });
            $scope.mdlActivity = "";
            $scope.mdlActivityPublished = "";
            //      $scope.mdlInternActivityType = "";
        }
        else {
            alert("Point/Type is mandatory.");
        }
    }


    $scope.deleteActivityArray = [];

    // Delete Intern Activity By Dlshad A. on 26 Aug 2020
    $scope.DeleteActivity = function (index) {

        var data2 = $scope.arrActivity[index];
        $scope.deleteActivityArray.push({ InternActivityId: data2.InternActivityId, ActivityPoint: data2.ActivityPoint });
        $scope.arrActivity.splice(index, 1);


        //      var data = $scope.arrActivity[index];
        //      $scope.arrActivity.splice(index, 1);
    }

    // Intership structure Preview By Dilshad on 28 Aug 2020
    $scope.InterStucturePreview = function () {
        $("#interPreview").modal("show");
    };

    // Add Activity details based on Add update for Student /Tutor By Dilshad A.
    $scope.AddActivity = function (Id, UserType, Opr, Msg, Desc) {
        $scope.clsAct = {};
        $scope.clsAct.Id = Id;
        $scope.clsAct.UserType = UserType;
        $scope.clsAct.ActivityOperation = Opr;
        $scope.clsAct.ActivityMessage = Msg;
        $scope.clsAct.ActivityDescription = Desc;
        $http.post("/Admin/AddActivity/", { objActivity: $scope.clsAct }).then(function (resAct) {
            if (resAct.data > 0) {
                console.log("Activity Added.");
            }
            else {
                console.log(resAct.data);
            }
        });
    }



    //  Students Details Show Init ( Init script for view students to Admin )

    $scope.studentDetailsShowInit = function () {
        $http.get('/Admin/GetStudentsWithCourse').then(function (response) {
            $scope.data = response.data;
            //$scope.ProfileImage = $scope.data[0].ProfileImage;
            $scope.StudentDetails = new NgTableParams({}, { dataset: $scope.data });
        });
    }


    // Students Details Save On Edit click in Session

    $scope.editStudentsDetail = function (user) {
        sessionStorage.StudentEditData = JSON.stringify(user);
        location.href = "/Admin/UpdateStudent";
    }



    // Student Details Edit By Admin
    // Students Update Init Page
    $scope.StudentEditDataOnInit = function () {

        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategoriesData = response.data;
        });

        if (sessionStorage.StudentEditData != undefined && sessionStorage.StudentEditData != '') {
            $scope.Id = JSON.parse(sessionStorage.StudentEditData);



            $http.get(`/Admin/GetStudentEditedData?StudentId=${$scope.Id}`).then(function (response) {
                $scope.data = response.data;
                $scope.EditedImage = true;
                $scope.Id = $scope.data[0].StudentId;
                $scope.Name = $scope.data[0].Name;
                $scope.PaymentStatus = $scope.data[0].PaymentStatus;
                $scope.momentDOB = $scope.data[0].DOB;
                $scope.Username = $scope.data[0].Username;
                $scope.Phone = $scope.data[0].Mobile;
                $scope.Email = $scope.data[0].Email;
                $scope.Username = $scope.data[0].Username;
                $scope.Title = $scope.data[0].ProfileImage;

                if ($scope.data[0].Fblink != undefined && $scope.data[0].Fblink != "null") {
                    $scope.Fblink = $scope.data[0].Fblink;
                }

                if ($scope.data[0].Instalink != undefined && $scope.data[0].Instalink != "null") {
                    $scope.InstaLink = $scope.data[0].Instalink;
                }

                if ($scope.data[0].Twitterlink != undefined && $scope.data[0].Twitterlink != "null") {
                    $scope.TwitterLink = $scope.data[0].Twitterlink;
                }

                $scope.PaidAmount = $scope.data[0].PaidAmount;
                $scope.DueAmount = $scope.data[0].Due;
                $scope.TemporaryRegNo = $scope.data[0].TemporaryRegNo;
                $scope.FinalRegNo = $scope.data[0].FinalRegNo;
                $scope.dtNextInstallmentDate = $scope.data[0].NextInstallmentDate;
                if ($scope.data[0].Remarks != "null" && $scope.data[0].Remarks != "") {
                    $scope.RemarksForCourse = $scope.data[0].Remarks;
                }

                if ($scope.data[0].RemarksPayment != "null" && $scope.data[0].RemarksPayment != "") {
                    $scope.PaymentRemarks = $scope.data[0].RemarksPayment;
                }


                $scope.dtNextInstallmentDate = $scope.data[0].NextInstallmentDate;
                $scope.paymentmode = $scope.data[0].PaymentMode;
                $scope.CoursesId = $scope.data[0].CourseId;
                $scope.Course = $scope.data[0].CourseName;
                $scope.Duration = $scope.data[0].Duration;
                $scope.JoiningDate = $scope.data[0].JoiningDate;
                $scope.Month = $scope.data[0].Month;
                $scope.NetAmount = $scope.data[0].NetAmountToPay;

                if ($scope.data[0].State != "null" && $scope.data[0].State != "") {
                    $scope.State = $scope.data[0].State;
                }

                // $scope.State = $scope.data[0].State;

                if ($scope.data[0].City != "null" && $scope.data[0].City != "") {
                    $scope.City = $scope.data[0].City;
                }

                //      $scope.City = $scope.data[0].City;

                if ($scope.data[0].Pincode != "null" && $scope.data[0].Pincode != "") {
                    $scope.PinCode = $scope.data[0].Pincode;
                }

                //  $scope.PinCode = $scope.data[0].Pincode;

                if ($scope.data[0].Address != "null" && $scope.data[0].Address != "") {
                    $scope.Address = $scope.data[0].Address;
                }


                if ($scope.data[0].Landmark != "null" && $scope.data[0].Landmark != "") {
                    $scope.LandMark = $scope.data[0].Landmark;
                }

                //   $scope.Address = $scope.data[0].Address;

                //  $scope.LandMark = $scope.data[0].Landmark;
                $scope.StudentCategoryId = $scope.data[0].StudentCategoryId;

                $scope.Total = null;
                $http.get(`/Admin/EditMultipleCourseDetails?StudentId=${$scope.Id}`).then(function (response) {
                    $scope.multipleCourse = response.data;
                    var i = 0;
                    while (i < $scope.multipleCourse.length) {
                        $scope.Total += $scope.multipleCourse[i].NetAmount;
                        $scope.Total = Math.floor($scope.Total);
                        i++;
                    }
                })
            })
        }
    }


    // Due Amount Calculation according to Paid Amount

    $scope.forDuesAmountCalculation = function () {
        $scope.DueAmount = $scope.Total - $scope.PaidAmount;
    }


    $scope.changeImage = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;

        var files = $scope.fullfile;
        var title = files.$ngfName;
        var reader = new FileReader();
        if (files) {
            reader.readAsDataURL(files);
            var name = files.name;
        }

        reader.addEventListener("load", function () {
            // $scope.Preview = reader.result;
            $scope.Preview = reader.result;
            $scope.EditedImage = false;
            $scope.PreviewImage = true;

        }, false);

        if ($scope.Preview !== undefined) {
            $scope.Preview = name;
            $scope.Title = $scope.Preview;
        }

    }


    // Update Student details payment etc For Admin

    $scope.updateStudentsDetails = function () {

        if ($scope.Phone != undefined) {
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;
            if (!$scope.mobilepatterns.test($scope.Phone)) {
                alert("10 digits only Mobile No must start with 6 or 7 or 8 or 9");
            }
        }

        if ($scope.Email != undefined) {
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            if (!$scope.emailpattern.test($scope.Email)) {
                alert("Invalid Email");
            }
        }

        if ($scope.Name != undefined) {
            $scope.Namepattern = /^[a-z A-Z]+$/;
            if (!$scope.Namepattern.test($scope.Name)) {
                alert("Name must contain alphabets A-Z and a-z only");
            }
        }

        if ($scope.Name != undefined && $scope.Username != undefined && $scope.Phone != undefined && $scope.Email != undefined && $scope.DOB != undefined && $scope.PaidAmount != undefined &&
            $scope.PaymentStatus != undefined && $scope.FinalRegNo != undefined) {

            $scope.Namepattern = /^[a-z A-Z]+$/;
            $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
            $scope.mobilepatterns = /^[6-9][0-9]{9}$/;

            if ($scope.Namepattern.test($scope.Name) && $scope.emailpattern.test($scope.Email) && $scope.mobilepatterns.test($scope.Phone)) {
                $scope.updatestudents = {};
                $scope.updatestudents.Name = $scope.Name;
                $scope.DOB = moment($scope.DOB).format("YYYY-MM-DD");
                $scope.updatestudents.FinalRegNo = $scope.FinalRegNo;
                $scope.updatestudents.PaymentMode = $scope.paymentmode;
                $scope.updatestudents.Username = $scope.Username;
                $scope.updatestudents.Email = $scope.Email;
                $scope.updatestudents.Fblink = $scope.Fblink;
                $scope.updatestudents.InstaLink = $scope.TwitterLink;
                $scope.updatestudents.Mobile = $scope.Phone;
                $scope.updatestudents.StudentCategoryId = $scope.StudentCategoryId;

                //if ($scope.Address != undefined && $scope.State != undefined && $scope.City != undefined && $scope.Pincode != undefined) {
                //    $scope.Address = $scope.Address;
                //    $scope.State = $scope.State;
                //    $scope.City = $scope.City;
                //    $scope.Pincode = $scope.Pincode;
                //}

                //else if ($scope.Address == undefined && $scope.State != undefined && $scope.City != undefined && $scope.Pincode != undefined) {
                //    alert("Address is required");
                //}

                //else if ($scope.Address != undefined && $scope.State == undefined && $scope.City != undefined && $scope.Pincode != undefined) {
                //    alert("State is required");
                //}

                //else if ($scope.Address != undefined && $scope.State != undefined && $scope.City == undefined && $scope.Pincode != undefined) {
                //    alert("City is required");
                //}

                //else if ($scope.Address != undefined && $scope.State != undefined && $scope.City != undefined && $scope.Pincode == undefined) {
                //    alert("Pin Code is required");
                //}


                //else if ($scope.Address != undefined && $scope.State == undefined && $scope.City == undefined && $scope.Pincode == undefined) {
                //    alert("State,City,PinCode is required");
                //}

                //else if ($scope.Address == undefined && $scope.State != undefined && $scope.City == undefined && $scope.Pincode == undefined) {
                //    alert("Address,City,PinCode is required");
                //}

                //else if ($scope.Address == undefined && $scope.State == undefined && $scope.City != undefined && $scope.Pincode == undefined) {
                //    alert("Address,State,PinCode is required");
                //}

                $scope.LandMark = $scope.LandMark;
                $scope.CourseRemarks = $scope.RemarksForCourse;
                $scope.updatestudents.RemarksPayment = $scope.PaymentRemarks;
                $scope.updatestudents.NextInstallmentDate = moment($scope.NextInstallmentDate).format("YYYY-MM-DD");
                $scope.updatestudents.FinalRegNo = $scope.FinalRegNo;
                $scope.updatestudents.PaymentMode = $scope.paymentmode;
                $scope.updatestudents.PaymentStatus = $scope.PaymentStatus;
                $scope.updatestudents.PaidAmount = $scope.PaidAmount;
                if ($scope.DueAmount != undefined && $scope.DueAmount != '') {
                    $scope.updatestudents.Due = $scope.DueAmount;
                }

                Upload.upload({
                    url: "/Admin/UpdateStudentsProfile",
                    data: {
                        upd: $scope.updatestudents,
                        postedfile: $scope.file,
                        StudentId: $scope.Id,
                        DOB: $scope.DOB,
                        Address: $scope.Address,
                        State: $scope.State,
                        City: $scope.City,
                        Pincode: $scope.PinCode,
                        LandMark: $scope.LandMark,
                        Remarks: $scope.CourseRemarks,
                        PaymentRemarks: $scope.PaymentRemarks
                    }
                }).then(function (response) {
                    $scope.data = response.data;
                    $scope.showLoader = false;
                    if (response.data == "Updated") {
                        alert("Data Updated Successfully");

                        // $("#updateprofileModal").modal('show');
                        location.href = "/Admin/ViewStudents";
                    }

                    else if (response.data == "DateBirth") {
                        swal({
                            title: 'Error!',
                            text: 'Age can not be less than 18',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }

                    else if (response.data == "Error") {
                        // alert("Problem while updating data");

                        swal({
                            title: 'Error!',
                            text: 'Can not Update Records! Please Contact Admin',
                            icon: "warning",
                            dangerMode: "true"
                        });
                    }
                })
            }
            else {
                swal({
                    title: 'Error!',
                    text: 'Please check all required fields are fill and validated',
                    icon: "warning",
                    dangerMode: "true"
                });
            }
        }
    }


    // Get Course List taken by student  
    $scope.CourseListDetail = function () {
        $http.get("/User/CourseListDetail").then(function (resCourseList) {
            $scope.CourseList = resCourseList.data;
        });
    }

    // View Activities By Dilshad A.
    $scope.ViewActivityDetails = function () {
        $http.get("/Admin/ViewActivityDetails").then(function (resViewAct) {
            $scope.Activities = resViewAct.data;
            $scope.ActivitiesTable = new NgTableParams({}, { dataset: $scope.Activities });
        });
    };


    // TutorPersonalProfileDetaisl by Dilshad A.
    $scope.TutorPersonalProfileDetails = function () {
        $scope.emailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        $scope.ImgOnlyOnEdit = true;
        $scope.PreviewImage = false;

        $http.post("/User/TutorPersonalProfileDetails").then(function (resTutor) {
            var t = resTutor.data;
            $scope.Id = t.TutorId;
            $scope.Title = t.TutorImage;
            $scope.Name = t.TutorName;
            $scope.Contact = t.TutorContact;
            $scope.Experience = t.TutorExperience;
            $scope.Pin = t.TutorPinCode;
            $scope.State = t.TutorState;
            if (t.ShortDescription != '' && t.ShortDescription != undefined && t.ShortDescription != null) {
                $scope.mdlShortDesc = t.ShortDescription;
            }

            if (t.LongDescription != '' && t.LongDescription != undefined && t.LongDescription != null) {
                $scope.mdlLongDesc = t.LongDescription;
            }

            $scope.arrIdUserType = [];
            $scope.arrIdUserType.push({ Id: $scope.Id, UserType: "All-Intern-Tutor" });
            sessionStorage.setItem("InternIdUserNameForTut", JSON.stringify($scope.arrIdUserType));

            //$scope.momentDOB = t.TutorDOB;
            $scope.momentDOB = moment(t.TutorDOB).format("DD/MM/YYYY");
            $scope.Email = t.TutorEmail;
        });
    };


    $scope.GetTutorCommunicationById = function (UserTye) {
        if (UserTye === 'All-Intern-Tutor') {
            var d = sessionStorage.getItem("InternIdUserNameForTut");
            d = JSON.parse(d);
            $http.post("/User/GetCommunicationById", { intId: d[0].Id, strUserType: d[0].UserType }).then(function (res) {
                $scope.CommByUserId = res.data.data;
                $scope.CommForAll = res.data.msgAll;

                //$scope.CommTutorsByUserId = res.data.data;
                //$scope.InternStudentsMsg = new NgTableParams({}, { dataset: $scope.CommTutorsByUserId });

                //$scope.CommTutorForAll = res.data.msgAll;
            });
        }
    };


    // Validate DOB in Tutor Personal Profile

    $scope.validateTutorPersonalProfileDOB = function () {

        if ($scope.DOB !== undefined) {
            var a = moment();
            var b = moment($scope.DOB);
            var yearsage = a.diff(b, 'years');
            var age = yearsage;
            if (age < 18) {
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 18',
                    icon: "warning",
                    dangerMode: "true"
                })
                //$scope.registration.$invalid = true;
            }
            else {

            }
        }
    }

    // Tutor Personal Profile Update by Atul Sh. 3 Sept.

    $scope.UpdateTutorPersonalProfile = function () {

        $scope.UpdateTutorsPersonalProfile = {};
        $scope.UpdateTutorsPersonalProfile.TutorName = $scope.Name;
        $scope.UpdateTutorsPersonalProfile.TutorContact = $scope.Contact;
        $scope.UpdateTutorsPersonalProfile.TutorExperience = $scope.Experience;
        $scope.UpdateTutorsPersonalProfile.TutorPinCode = $scope.Pin;
        $scope.UpdateTutorsPersonalProfile.TutorState = $scope.State;
        $scope.UpdateTutorsPersonalProfile.TutorDOB = moment($scope.DOB).format("YYYY-MM-DD");
        $scope.UpdateTutorsPersonalProfile.TutorEmail = $scope.Email;
        $scope.UpdateTutorsPersonalProfile.ShortDescription = $scope.mdlShortDesc;
        $scope.UpdateTutorsPersonalProfile.LongDescription = $scope.mdlLongDesc;

        Upload.upload({
            url: "/User/UpdateTutorPersonalProfile",
            data: {
                Id: $scope.Id,
                update: $scope.UpdateTutorsPersonalProfile,
                postedfile: $scope.fullfile
            }
        }).then(function (response) {
            $scope.data = response.data;

            if (parseInt(response.data) > 0) {
                $("#updateTutorProfileModal").modal('show');
            }
            else {
                alert("Error while updating");
            }
        });
    }


    $scope.uploadTutorImage = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;

        var files = $scope.fullfile;
        var title = files.$ngfName;
        var reader = new FileReader();
        if (files) {
            reader.readAsDataURL(files);
            var name = files.name;
        }

        reader.addEventListener("load", function () {
            // $scope.Preview = reader.result;
            $scope.Preview = reader.result;
            $scope.ImgOnlyOnEdit = false;
            $scope.PreviewImage = true;

        }, false);

        if ($scope.Preview !== undefined) {
            $scope.Preview = name;
            $scope.Title = $scope.Preview;
        }
    }


    //  Admin/AddDuration (Dynamic Duration)

    $scope.adddurationinit = function () {
        $http.get('/Admin/GetAddedDurations').then(function (response) {
            $scope.data = response.data;
            $scope.ShowDuration = new NgTableParams({}, { dataset: $scope.data });
        })
    }


    // Function for Create Duration

    $scope.saveDuration = function () {

        if (sessionStorage.Duration !== undefined && $scope.strDuration !== undefined) {
            $scope.data3 = JSON.parse(sessionStorage.Duration);

            if ($scope.data3 !== undefined && $scope.data3 != '') {
                $scope.DurationUpdate = {};
                $scope.DurationUpdate.DurationId = $scope.data3.DurationId;
                $scope.DurationUpdate.DurationName = $scope.strDuration;

            }


            $http.post('/Admin/UpdateDuration', { update: $scope.DurationUpdate }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    sessionStorage.clear();
                    alert("Duration Updated Successfully");
                    window.location.href = "/Admin/AddDuration";
                }
                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("Duration already exists");
                }
                else {
                    alert("Error! Problem while Updating Duration");
                }
            })

        }
        else {
            $scope.Duration = {};
            $scope.Duration.DurationName = $scope.strDuration;

            $http.post('/Admin/CreateDuration', { objDuration: $scope.Duration }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    alert("Duration created successfully");
                    window.location.href = "/Admin/AddDuration";
                }
                else if (response.data == false) {
                    alert("Duration already exists");
                }
                else if (response.data == "empty") {
                    swal({
                        title: 'Error',
                        text: 'Duration is mandatory',
                        icon: "warning",
                        dangerMode: "true"
                    });
                    // alert("Please enter Course Course is mandatory");
                }

                else {
                    alert("Error! Problem while creating Duration");
                }
            })
        }
    }


    $scope.editDuration = function (Id) {
        $scope.Id = Id;
        sessionStorage.Duration = JSON.stringify($scope.Id);

        if (sessionStorage.Duration !== undefined && sessionStorage.Duration != '') {
            $scope.data2 = JSON.parse(sessionStorage.Duration);
        }

        if ($scope.data2 !== undefined && $scope.data2 != '') {
            $scope.strDuration = $scope.data2.DurationName;
        }

    }

    $scope.deleteDuration = function (Id) {
        $scope.Id = Id;
    }



    // Delete Duration 

    $scope.DeleteDuration = function (Id) {
        $http.post('/Admin/DeleteDuration', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;

            if (parseInt(response.data) > 0) {
                alert("Data Deleted Successfully");
                window.location.href = "/Admin/AddDuration";
            }

            else {
                alert("Error while deleting duration");
            }

        })
    }


    // Internship Activity Type Init

    $scope.addinternshipactivitytypeinit = function () {
        $http.get('/Admin/GetAddedInternshipType').then(function (response) {
            $scope.data = response.data;
            $scope.InternshipActivityType = new NgTableParams({}, { dataset: $scope.data });
        })
    }


    $scope.editInternshipActivityType = function (Id) {
        $scope.Id = Id;
        sessionStorage.InternActivityType = JSON.stringify($scope.Id);

        if (sessionStorage.InternActivityType !== undefined && sessionStorage.InternActivityType != '') {
            $scope.data2 = JSON.parse(sessionStorage.InternActivityType);
        }

        if ($scope.data2 !== undefined && $scope.data2 != '') {
            $scope.strInternshipActivityType = $scope.data2.InternActivityType;
        }
    }

    // Internship Activity Type Create / Update

    $scope.saveInternshipActivity = function () {

        if (sessionStorage.InternActivityType !== undefined && $scope.strInternshipActivityType !== undefined) {
            $scope.data3 = JSON.parse(sessionStorage.InternActivityType);

            if ($scope.data3 !== undefined && $scope.data3 != '') {
                $scope.InternActivityTypeUpdate = {};
                $scope.InternActivityTypeUpdate.InternActivityTypeId = $scope.data3.InternActivityTypeId;
                $scope.InternActivityTypeUpdate.InternActivityType = $scope.strInternshipActivityType;
            }

            $http.post('/Admin/UpdateInternActivityType', { update: $scope.InternActivityTypeUpdate }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    sessionStorage.clear();
                    alert("Internship Activity Type Updated Successfully");
                    window.location.href = "/Admin/AddInternActivityType";
                }
                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("Internship Activity Type already exists");
                }
                else {
                    alert("Error! Problem while creating Internship Activity Type");
                }
            })

        }
        else {
            $scope.tblInternActivityType = {};
            $scope.tblInternActivityType.InternActivityType = $scope.strInternshipActivityType;

            $http.post('/Admin/CreateInternshipActivityType', { objInternActivityType: $scope.tblInternActivityType }).then(function (response) {
                $scope.data = response.data;

                if (response.data == true) {
                    alert("Internship Activity Type created successfully");
                    window.location.href = "/Admin/AddInternActivityType";
                }
                else if (response.data == false) {
                    alert("Internship Activity Type already exists");
                }
                else if (response.data == "empty") {
                    swal({
                        title: 'Error',
                        text: 'Internship Activity Type is mandatory',
                        icon: "warning",
                        dangerMode: "true"
                    });
                    // alert("Please enter Course Course is mandatory");
                }

                else {
                    alert("Error! Problem while creating Internship Activity Type");
                }
            })
        }

    }

    $scope.deleteInternshipActivity = function (Id) {
        $scope.Id = Id;
    }


    // Delete Internship Activity Type Duration 
    $scope.DeleteInternshipActivityType = function (Id) {
        $http.post('/Admin/DeleteInternshipActivityType', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;

            if (parseInt(response.data) > 0) {
                alert("Data Deleted Successfully");
                window.location.href = "/Admin/AddInternActivityType";
            }

            else {
                alert("Error while deleting Intern Activity Type");
            }
        })
    }

    // Upload Aadhar for Inter Applied Student/Professional By Dilshad A. on 09 Sept 2020
    $scope.uploadAadhaarForIntern = function (file) {
        $scope.aadhaarFileName = file.$ngfName;
        $scope.aadhaarFullfile = file;
    }

    //Upload Resume For Intern Applied Student/Professional By Dilshad A. on 09 Sept 2020
    $scope.uploadResumeForIntern = function (file) {
        $scope.resumeFileName = file.name;
        $scope.resumeFullfile = file;
    }

    //Upload Image For Intern Applied Student/Professional By Dilshad A. on 09 Sept 2020
    $scope.uploadImageForIntern = function (file) {
        $scope.imgFileName = file.$ngfName;
        $scope.imgFullfile = file;
    }

    // Add Intern Apply Student Details By Dilshad A. on 09 Sept 2020
    $scope.AddApplyForInternship = function () {
        if (sessionStorage.InternshipId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.InternshipId);

            $scope.clsInternApply = {};
            $scope.clsInternApply.InternshipStructureId = $scope.Id;
            $scope.clsInternApply.Name = $scope.mdlInternApplyName;
            $scope.clsInternApply.DOB = moment($scope.mdlInternApplyDOB).format("YYYY-MM-DD");
            $scope.clsInternApply.Email = $scope.mdlInternApplyEmail;
            $scope.clsInternApply.Twiter = $scope.mdlInternApplyTwitter;
            $scope.clsInternApply.Instagram = $scope.mdlInternApplyInsta;
            $scope.clsInternApply.Facebook = $scope.mdlInternApplyFacebook;
            $scope.clsInternApply.HowDoYouKnow = $scope.mdlInternApplyHowDoYouKnow;
            $scope.clsInternApply.Image = $scope.imgFileName;
            $scope.clsInternApply.CollegeUniv = $scope.mdlInternApplyCollege;
            $scope.clsInternApply.Qualification = $scope.mdlInternApplyQualification;
            $scope.clsInternApply.Trade = "-";
            $scope.clsInternApply.YearOfPassing = $scope.mdlInternApplyYOP;

            /*================Contact Details=================*/
            $scope.clsContact = {};
            $scope.clsContact.Mobile = $scope.mdlInternApplyContact;
            $scope.clsContact.Email = $scope.mdlInternApplyEmail;
            $scope.clsContact.Address = $scope.mdlInternApplyAddress;
            $scope.clsContact.State = $scope.mdlInternApplyState;
            $scope.clsContact.City = "-";
            $scope.clsContact.Pincode = $scope.mdlInternApplyPin;

            $scope.arrInterApplyDoc = [];
            if ($scope.resumeFileName !== undefined && $scope.resumeFileName != "") {
                $scope.arrInterApplyDoc.push({ DocumentName: $scope.resumeFileName, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
            }

            if ($scope.mdlInternApplyAadhar !== undefined && $scope.mdlInternApplyAadhar != "") {
                $scope.arrInterApplyDoc.push({ DocumentName: $scope.aadhaarFileName, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
            }
            Upload.upload({
                url: "/User/AddApplyForInternship",
                method: "POST",
                data: {
                    objApply: $scope.clsInternApply, lstDocument: $scope.arrInterApplyDoc, objContact: $scope.clsContact,
                    imgPostedfile: $scope.imgFullfile, resumePostedfile: $scope.resumeFullfile, aadhaarPostedfile: $scope.aadhaarFullfile
                }
            }).then(function (resInterApply) {
                if (parseInt(resInterApply.data) > 0) {
                    $scope.AddActivity(parseInt(resInterApply.data), "Inter Student", "Intern for Intern", "New Intern Registered", "Intern Student Registered Desc");
                    alert("You have successfully registered.");
                    window.location.href = "/User/InternshipList";
                }
                else {
                    alert("Failed! Something went wrong!");
                }
            });
        }
        else {
            alert("Something went wrong. Please try once from Internship List page.");
            window.locatio.href = "/User/InternshipList";
        }
    }

    // Call Internship registration page  By Dilshad A. on 09 Sept 2020
    $scope.InternApplyPage = function () {
        window.location.href = "/User/ApplyInternship";
    };

    // Intern Student Details By Dilshad A. on 10 Sept 2020
    $scope.InternStudentDetails = function () {
        $http.get("/Admin/InternStudentDetails").then(function (resIntern) {
            $scope.InternStudents = resIntern.data.internWithContact;
            $scope.InternStudentsTable = new NgTableParams({}, { dataset: $scope.InternStudents });
        });
    };

    // Internship Structure Show BAsed on published by Dilshad A. on 10 Sept 2020
    $scope.GetPublishedInternshipStructureDetails = function () {
        $http.get('/Internship/GetPublishedInternshipStructureDetails').then(function (response) {
            $scope.data = response.data;
            $scope.InternshipStrTable = new NgTableParams({}, { dataset: $scope.data });
        })
    }


    $scope.InternListTutorinit = function () {

        $http.get('/Admin/InterListTutorsData').then(function (response) {
            $scope.InternTutorList = response.data;

            //  $scope.InterListTutorsData = new NgTableParams({}, { dataset: $scope.data });
        })

    }

    $scope.getInternTutorDetails = function (Id) {

        $scope.Id = Id;
        sessionStorage.InternshipsId = JSON.stringify($scope.Id);
        location.href = "/Internship/AddTutorResourcesForInternship";
    }


    $scope.InternTutorArray = [];
    $scope.getInternTutorDetailsOnId = function () {
        if (sessionStorage.InternshipsId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.InternshipsId);
        }

        $http.get(`/Internship/GetTutorResourcesForInternship?InternshipId=${$scope.Id}`).then(function (response) {
            $scope.data = JSON.parse(response.data);

            if ($scope.data["intern"] != '' && $scope.data["intern"] != undefined) {
                $scope.InternshipName = $scope.data["intern"][0].InternshipName;
            }

            $scope.InternshipArray = [];

            if ($scope.data["intrtutor"] != '' && $scope.data["intrtutor"] != undefined) {

                for (var i = 0; i < $scope.data["intrtutor"].length; i++) {
                    $scope.InternshipArray.push($scope.data["intrtutor"][i]);
                }
            }

            $scope.filesarray = [];
            if ($scope.data["resource"] != '' && $scope.data["resource"] != undefined) {
                for (var i = 0; i < $scope.data["resource"].length; i++) {
                    $scope.filesarray.push({ ResourceName: $scope.data["resource"][i].ResourceName, ResourceId: $scope.data["resource"][i].ResourceId, InternshipId: $scope.data["resource"][i].InternshipId });
                }
            }



        });


        $http.get("/Admin/GetTutorsData").then(function (response) {
            $scope.TutorsData = response.data;
        })
    }



    $scope.InternshipArray = [];
    $scope.AddMultipleTutorForInternship = function () {
        $scope.InternshipArray.push({ InternshipId: $scope.Id, InternshipName: $scope.InternshipName, TutorId: $scope.mdlTutor.TutorId, TutorName: $scope.mdlTutor.TutorName });

    }


    $scope.deleteInternTutorArray = [];

    $scope.DeleteInternTutorArray = function (index) {

        var data2 = $scope.InternshipArray[index];
        $scope.deleteInternTutorArray.push({ InternshipId: data2.InternshipId, TutorId: data2.TutorId, InternTutorId: data2.InternTutorId });
        $scope.InternshipArray.splice(index, 1);
    }


    $scope.deleteInternResourceArray = [];
    $scope.DeleteInternResourceArray = function (index) {

        var data2 = $scope.filesarray[index];
        $scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.filesarray.splice(index, 1);
    }



    // Add Multiple Tutor Resources For Internship

    $scope.AddTutorResourceForInternship = function () {

        $http.post('/Internship/AddTutorForInternship', { InternshipArray: $scope.InternshipArray }).then(function (response) {
            $scope.data = response.data;
        })
    }





    $scope.ImageArray = [];



    // Add Multiple Tutor Resources For Internship

    $scope.AddTutorResourceForInternship = function () {
        Upload.upload({
            url: "/Internship/AddTutorForInternship",
            method: "POST",
            data: {
                InternshipArray: $scope.InternshipArray,
                Filearray: $scope.filesarray,
                InternshipId: $scope.Id,
                deleteinterntutor: $scope.deleteInternTutorArray,
                deleteInternResourceArray: $scope.deleteInternResourceArray
            }
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == "Done") {
                alert("Data Inserted Successfully");
                location.href = "/Admin/InternListTutor";
            }
        })
    }



    // Multiple files Upload 16 September

    $scope.UploadFiles = function (files) {

        $scope.SelectedFiles = files;

        for (var i = 0; i < $scope.SelectedFiles.length; i++) {
            if ($scope.SelectedFiles[i].type == "application/x-msdownload" || $scope.SelectedFiles[i].type == "video/mp4") {
                alert("Can't upload video and exe");
            }
            else {
                //  $scope.Filearray.push($scope.SelectedFiles[i].name);            
                $scope.filesarray.push({ Files: $scope.SelectedFiles, ResourceName: $scope.SelectedFiles[i].name, InternshipId: $scope.Id });
                Upload.upload({
                    url: "/Internship/Upload",
                    data: {
                        files: $scope.SelectedFiles
                    }
                }).then(function (response) {
                    $scope.data = response.data;

                });
            }
        }
    }




    // On Going Internship Init
    $scope.OnGoingInternshipinit = function () {

        $http.get('/Internship/OnGoingInternshipDetails').then(function (response) {
            $scope.data = response.data;
            $scope.OngoingInternshipData = new NgTableParams({}, { dataset: $scope.data });
        })
    }

    // View Communication Details
    $scope.ViewCommunicationDetails = function () {
        $http.get("/User/ViewCommunicationDetails").then(function (resComm) {
            $scope.Communication = resComm.data;
        });
    };

    // Intern Student Details By Dilshad A. on 10 Sept 2020 and aslo used at time of adding user
    $scope.InternStudentDetails = function () {
        $http.get("/Admin/InternStudentDetails").then(function (resIntern) {
            $scope.InternStudents = resIntern.data.internWithContact;
            $scope.InternStudentsTable = new NgTableParams({}, { dataset: $scope.InternStudents });
        });
    };

    // Internship Structure Show BAsed on published by Dilshad A. on 10 Sept 2020
    $scope.GetPublishedInternshipStructureDetails = function () {
        $http.get('/Internship/GetPublishedInternshipStructureDetails').then(function (response) {
            $scope.data = response.data;
            $scope.InternshipStrTable = new NgTableParams({}, { dataset: $scope.data });
        });
    };

    // ===================Angucomplete start here by Dilshad A. on 12 Sept 2020===================
    $scope.arrStudentsAngu = [];
    $scope.afterNameSelect = function (selected) {
        if (selected !== undefined) {
            $scope.mdlStudentNameAngu = selected.originalObject.NameTitleField + selected.originalObject.EmailTitleField;
        }
    };

    // Get Student Name and Email by Dilshad A. on 12 Sept 2020 for Angu Complete
    // Get Student Name and Email by Dilshad A. on 12 Sept 2020 for Angu Complete
    $scope.GetStudentDetails = function () {
        $http.get("/Admin/GetStudentDetails").then(function (res) {
            $scope.students = res.data;
            for (var i = 0; i < res.data.length; i++) {
                $scope.arrStudentsAngu.push({ NameTitleField: res.data[i].Name + "-", EmailTitleField: res.data[i].Email + "-", StudentId: res.data[i].StudentId });
            }
        });
    }

    $scope.localSearch = function (str) {
        var matches = [];
        var d = $scope.arrStudentsAngu;
        d.forEach(function (d) {
            if ((d.NameTitleField.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0) &&
                (d.EmailTitleField.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0)) {
                matches.push(d);
            }
        });
        return matches;
    }

    // Add multi Students for communication BY Dilshad on 12 Sept 2020
    $scope.arrStudents = [];
    $scope.AddMultipleStudent = function () {
        var count = 0;
        if ($scope.mdlStudentNameAngu != undefined && $scope.mdlStudentNameAngu != "") {
            var arrData = $scope.mdlStudentNameAngu.split('-');
            for (var i = 0; i < $scope.arrStudents.length; i++) {
                if ($scope.arrStudents[i].Name === arrData[0] && $scope.arrStudents[i].Email === arrData[1]) {
                    count++;
                }
            }
            if (count === 0) {
                $scope.arrStudents.splice(0, 0, { Name: arrData[0], Email: arrData[1] });
                //$scope.divNameCountrySCode = true;
                $scope.mdlStudentNameAngu = "";
                $scope.$broadcast('angucomplete-alt:clearInput');
            }
            else {
                alert("Name is already added!!!");
            }
        }
        else {
            alert('Please select Name.')
        }
    };

    // Delete Seleted DocNum
    $scope.deleteDocNumRow = function (index) {
        var d = $scope.arrStudents.indexOf($scope.arrStudents[index]);
        $scope.arrStudents.splice(d, 1);
        if ($scope.arrStudents.length <= 0) {
            $scope.divDocNum = false;
        }
    }
    // ===================Angucomplete END here By Dilshad A. on 12 Sept 2020===================

    // ===================Angucomplete start here for get Tutaor details by Dilshad A. on 14 Sept 2020===================
    $scope.arrTutorAngu = [];
    $scope.afterTutorNameSelect = function (selected) {
        if (selected !== undefined) {
            $scope.mdlTutorNameAngu = selected.originalObject.NameTitleField + selected.originalObject.EmailTitleField;
        }
    };

    // Get Tutor Name and Email by Dilshad A. on 14 Sept 2020 for Angu Complete    
    $scope.GetTutorDetails = function () {
        $http.get("/Admin/GetTuturDetails").then(function (res) {
            //$scope.students = res.data;
            for (var i = 0; i < res.data.length; i++) {
                $scope.arrTutorAngu.push({ NameTitleField: res.data[i].TutorName + "-", EmailTitleField: res.data[i].TutorEmail, TutorId: res.data[i].TutorId });
            }
        });
    }

    $scope.localSearchTutor = function (str) {
        var matches = [];
        var d = $scope.arrTutorAngu;
        d.forEach(function (d) {
            if ((d.NameTitleField.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0)) {
                matches.push(d);
            }
        });
        return matches;
    }

    // Add multi Tutors for communication BY Dilshad A. on 14 Sept 2020
    $scope.arrTutors = [];
    $scope.AddMultipleTutor = function () {
        var count = 0;
        if ($scope.mdlTutorNameAngu != undefined && $scope.mdlTutorNameAngu != "") {
            var arrData = $scope.mdlTutorNameAngu.split('-');
            for (var i = 0; i < $scope.arrTutors.length; i++) {
                if ($scope.arrTutors[i].Name === arrData[0] && $scope.arrTutors[i].Email === arrData[1]) {
                    count++;
                }
            }
            if (count === 0) {
                $scope.arrTutors.splice(0, 0, { Name: arrData[0], Email: arrData[1] });
                $scope.mdlTutorNameAngu = "";
                $scope.$broadcast('angucomplete-alt:clearInput');
            }
            else {
                alert("Name is already added!!!");
                $scope.mdlTutorNameAngu = "";
            }
        }
        else {
            alert('Please select Name.')
        }
    };

    // Delete Seleted DocNum
    $scope.deletTutorsRow = function (index) {
        var d = $scope.arrTutors.indexOf($scope.arrTutors[index]);
        $scope.arrTutors.splice(d, 1);
        if ($scope.arrTutors.length <= 0) {
            $scope.divDocNum = false;
        }
    }
    // ===================Angucomplete END here for Tutor details By Dilshad A. on 14 Sept 2020===================

    // Add Message For Communicatioin to Students/Tutors By Dilshad A. on 14 Sept 2020
    $scope.AddMessageForCommunicatioin = function () {

        var count = 0;
        if ($scope.chkAllStudents == undefined || $scope.chkAllStudents == "") {
            $scope.chkAllStudents = false;
        }

        if ($scope.chkAllTutors == undefined || $scope.chkAllTutors == "") {
            $scope.chkAllTutors = false;
        }

        if ($scope.mdlStudentClassCategory === undefined || $scope.mdlStudentClassCategory === "") {
            $scope.mdlStudentClassCategory = false;
        }

        if ($scope.mdlInternshipIdForStudent === undefined || $scope.mdlInternshipIdForStudent === "") {
            $scope.mdlInternshipIdForStudent = false;
        }

        if ($scope.chkAllInternStudents === undefined || $scope.chkAllInternStudents === "") {
            $scope.chkAllInternStudents = false;
        }

        if ($scope.chkAllCourseStudents === undefined || $scope.chkAllCourseStudents === "") {
            $scope.chkAllCourseStudents = false;
        }

        if ($scope.chkAllInternTutors === undefined || $scope.chkAllInternTutors === "") {
            $scope.chkAllInternTutors = false;
        }

        if ($scope.chkAllCourseTutors === undefined || $scope.chkAllCourseTutors === "") {
            $scope.chkAllCourseTutors = false;
        }

        if ($scope.chkAllBrandStudents === undefined || $scope.chkAllBrandStudents === "") {
            $scope.chkAllBrandStudents = false;
        }

        // lisof Student/Tutor valu empty to easly on action method based on list data
        if ($scope.chkAllInternStudents === undefined || $scope.chkAllInternStudents === "") {
            $scope.chkAllInternStudents = false;
            //$scope.arrStudents = [];
        }

        if ($scope.chkAllCourseStudents === true) {
            //$scope.arrStudents = [];
        }

        if ($scope.chkAllInternTutors === true) {
            $scope.arrTutors = [];
        }

        if ($scope.chkAllCourseTutors === true) {
            $scope.arrTutors = [];
        }

        $http.post("/Admin/AddMessageForCommunicatioin",
            {
                lstStudent: $scope.arrStudents, lstTutor: $scope.arrTutors, isAllStudents: $scope.chkAllStudents, isAllTutors: $scope.chkAllTutors,
                strMsgStudent: $scope.mdlMsgOnlyStudent, strMsgTutor: $scope.mdlMsgOnlyTutor, strMsgAll: $scope.mdlMsgAll,
                CategoryType: $scope.mdlCategoryTypeForStudent, StudentCategoryId: $scope.mdlStudentClassCategory, InternshipId: $scope.mdlInternshipIdForStudent,
                CourseId: $scope.mdlStudentClassCategoryId, IsAllInternStudent: $scope.chkAllInternStudents, IsAllCourseStudent: $scope.chkAllCourseStudents,
                IsAllInternTutor: $scope.chkAllInternTutors, IsAllCourseTutor: $scope.chkAllCourseTutors, lstCourseStudent: $scope.arrCourseStudents,
                lstBrandStudent: $scope.arrBrandStudentByAdmins, IsAllBrandStudent: $scope.chkAllBrandStudents
                //StudentCategoryId:$scope.
            }).then(function (resAddComm) {
                if (resAddComm.data > 0) {
                    alert("Message successfully sent.");
                    window.location.href = "/Admin/Communication";
                }
                else {
                    console.log(resAddComm.data);
                    alert("Failed. Something went wrong.");
                }
            });
    };

    // Script Communication Init by Atul 0n 17 Sept 2020
    $scope.showcommunicationinit = function () {
        $http.get("/Admin/GetCommunications").then(function (response) {
            $scope.data = response.data;
            $scope.ShowCommunication = new NgTableParams({}, { dataset: $scope.data });
        });
    }

    // View StudentCategory Details at the time Add Communication by Dilshad A. on 21 Sept 2020
    $scope.ViewStudentCategoryDetails = function () {
        //$http.get("/Admin/ViewStudentCategoryDetails").then(function (res) {
        //    $scope.StudentCategory = res.data;
        //})

        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategory = response.data;
        })

    }

    // View IternStudent By InternshipId by Dilshad A. on 21 Sept 2020
    $scope.ViewIternStudentByInternshipId = function (Id) {
        $http.post("/Admin/ViewIternStudentByInternshipId", { intIntershipId: Id }).then(function (res) {
            $scope.InterStudentListByInternShipId = res.data;
            $scope.InterStudentListByInternShipIdTable = new NgTableParams({}, { dataset: $scope.InterStudentListByInternShipId });
        });
    };

    $scope.shStudentClassCategory = false;
    $scope.shInternCourseCategoryType = false;
    $scope.shListOfInternStudent = false;
    // Show/Hide Section for intern/course By Category Type By Dilshad A. on 22 Sept 2020
    $scope.ViewSectionByCategoryTypeForStudent = function () {
        if ($scope.mdlCategoryTypeForStudent === "Intern") {
            $scope.shInternCourseCategoryType = true;
            $scope.shListOfInternStudent = true;
            $scope.shStudentClassCategory = false;
            $scope.shStudentCourseByStudentClassCategory = false;
        }
        else if ($scope.mdlCategoryTypeForStudent === "Course") {
            $scope.shInternCourseCategoryType = false;
            $scope.shStudentClassCategory = true;
            $scope.shListOfInternStudent = false;
        }
        else if ($scope.mdlCategoryTypeForStudent === "Brand") {
            $scope.shInternCourseCategoryType = false;
            $scope.shStudentClassCategory = true;
            $scope.shListOfInternStudent = false;
           // $scope.shStudentCourseByStudentClassCategory = false;
          //  $scope.shBrandStudentByStudentClassCategory = true;
        }

    };

    // View Student Course By Student Class Category(8-10,11-12,UG) By Dilshad A. On 25 Sept 2020 
    $scope.shStudentCourseByStudentClassCategory = false;
    $scope.ViewStudentCourseByStudentClassCategory = function () {
        if ($scope.mdlStudentClassCategory !== undefined && $scope.mdlStudentClassCategory !== "") {
            $scope.shStudentCourseByStudentClassCategory = true;
            $scope.catgid = $scope.mdlStudentClassCategory;
        }
        else {
            $scope.shStudentCourseByStudentClassCategory = false;
        }
    }

    //Select Multiple Intern Student For COmmunication By Dilshad A. on 22 Sept 2020
    $scope.arrInternStudent = [];
    $scope.SelectMultipleInternStudentForCommunication = function () {
        var data = $scope.InterStudentListByInternShipId;
        $scope.arrStudents = [];
        for (var i = 0; i < $scope.InterStudentListByInternShipId.length; i++) {
            if ($scope.InterStudentListByInternShipId[i].chkInternStudent == true) {
                $scope.arrStudents.push({ Name: data[i].Name, Email: data[i].Email, StudentId: data[i].InterApllyId });
            }
        }
    };


    //Select Multiple Brand Student For Admin To Brand Student Communication By Atul Sh.
    $scope.arrBrandStudentByAdmin = [];
    $scope.SelectMultipleBrandStudentForAdminCommunication = function () {
        var data = $scope.BrandStudentsAdminStudentComm;
        $scope.arrBrandStudentByAdmins = [];
        for (var i = 0; i < $scope.BrandStudentsAdminStudentComm.length; i++) {
            if ($scope.BrandStudentsAdminStudentComm[i].chkAdminToBrandStudent == true) {
                $scope.arrBrandStudentByAdmins.push({ Name: data[i].Name, Email: data[i].Email, StudentId: data[i].StudentId });
            }
        }
    };



    //Select Multiple Course Student For COmmunication By Dilshad A. on 22 Sept 2020
    $scope.arrCourseStudent = [];
    $scope.SelectMultipleCourseStudentForCommunication = function () {
        var data = $scope.CourseStudents;
        $scope.arrCourseStudents = [];
        for (var i = 0; i < $scope.CourseStudents.length; i++) {
            if ($scope.CourseStudents[i].chkCourseStudent == true) {
                $scope.arrCourseStudents.push({ Name: data[i].Name, Email: data[i].Email, StudentId: data[i].StudentId });
            }
        }
    };


    // Get Student details based on Course Id and Category Type
    $scope.GetStudentDetailsByCourseId = function (id, categorytype) {

        if ($scope.mdlCategoryTypeForStudent === "Brand") {
            $http.post("/Admin/GetBrandStudentsByCourseId", { intBrandCourseId: id, categoryid: $scope.catgid }).then(function (response) {
                $scope.BrandStudentsAdminStudentComm = response.data;

                $scope.BrandStudentListByCourseCategory = new NgTableParams({}, { dataset: $scope.BrandStudentsAdminStudentComm });
                $scope.shListOfBrandStudent = true;

            })
        }
        else {
        $http.post("/Admin/GetStudentsByCourseId", { intCourseId: id, categoryid: $scope.catgid }).then(function (response) {
            $scope.CourseStudents = response.data;

            $scope.StudentListByCourseCategory = new NgTableParams({}, { dataset: $scope.CourseStudents });
            $scope.shListOfCourseStudent = true;

            })
        }


    }


    //CourseId: 2067
    //CourseName: "Blazer"
    //Email: "atulsha628@gmail.com"
    //Name: "Abhay Sir"
    //StructureId: 2073
    //StudentCategoryId: 2
    //StudentId: 10019


    // Get Intern Tutor By IntershipId By Dilshad A. on 22 Sept 2020
    $scope.GetInternTutorByIntershipId = function (id) {
        $http.post("/Admin/GetInternTutorByIntershipId", { intInternshipId: id }).then(function (res) {
            $scope.InternTutors = res.data;
        });
    };

    $scope.shInternCourseCategoryTypeForTutor = false;

    //ViewSectionByCategoryTypeForTotor By Dilshad A. on 22 Sept 2020
    $scope.ViewSectionByCategoryTypeForTotor = function () {
        if ($scope.mdlCategoryTypeForTutor === "Intern") {
            $scope.shInternCourseCategoryType = true;
            $scope.shInternCourseCategoryTypeForTutor = true;
        }
        else if ($scope.mdlCategoryTypeForTutor === "Course") {
            $scope.shInternCourseCategoryTypeForTutor = false;
            $scope.shListOfInternStudent = false;
        }
    }

    // View Itern Tutor By InternshipId By Dilshad A. on 22 Sept 2020
    $scope.ViewIternTutorByInternshipId = function (Id) {
        $http.post("/Admin/GetInternTutorByIntershipId", { intInternshipId: Id }).then(function (res) {
            $scope.InterTutorListByInternShipId = res.data;
            $scope.InterTutorListByInternShipIdTable = new NgTableParams({}, { dataset: $scope.InterTutorListByInternShipId });
            $scope.shListOfInternTutor = true;
        });
    };

    // SelectMultipleInternTutorForCommunication By Dilshad A. on 23 Sept 2020
    $scope.SelectMultipleInternTutorForCommunication = function () {
        var data = $scope.InterTutorListByInternShipId;
        $scope.arrTutors = [];
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].chkInternTutor == true) {
                    $scope.arrTutors.push({ Name: data[i].TutorName, Email: data[i].TutorEmail, TutorId: data[i].TutorId });
                }
            }
        }
    };

    // GetCommunicationById By Dilshad A. 24 Sept 2020
    $scope.GetCommunicationById = function (UserTye) {
        if (UserTye === 'All-Intern-Students') {
            var d = sessionStorage.getItem("InternIdUserNameForCommu");
            d = JSON.parse(d);
            $http.post("/User/GetCommunicationById", { intId: d[0].Id, strUserType: d[0].UserType }).then(function (res) {
                $scope.InternStudentMsgByUserId = res.data.data;
                $scope.InternStudentMessagesByUserId = new NgTableParams({}, { dataset: $scope.InternStudentMsgByUserId });
                //  $scope.CommForAll = res.data.msgAll;
            });
        }
    };


    // Intern Profile Init

    $scope.internprofileinit = function () {

        $http.get('/User/ShowInternshipData').then(function (response) {
            $scope.data = response.data;

            $scope.emailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;;
            $scope.Id = $scope.data[0].InterApllyId;
            $scope.Name = $scope.data[0].Name;
            $scope.momentDOB = moment($scope.data[0].DOB).format("DD/MM/YYYY");
            $scope.Email = $scope.data[0].Email;

            if ($scope.data[0].Mobile != undefined) {
                $scope.Phone = $scope.data[0].Mobile;
            }

            if ($scope.data[0].Facebook != '' && $scope.data[0].Facebook != undefined) {
                $scope.Facebook = $scope.data[0].Facebook;
            }

            if ($scope.data[0].Instagram != '' && $scope.data[0].Instagram != undefined) {
                $scope.Instagram = $scope.data[0].Instagram;
            }

            if ($scope.data[0].Twiter != '' && $scope.data[0].Twiter != undefined) {
                $scope.Twitter = $scope.data[0].Twiter;
            }

            $scope.Username = $scope.data[0].Username;
            $scope.Id = $scope.data[0].StudentId;
            $scope.State = $scope.data[0].State;
            $scope.City = $scope.data[0].City;
            $scope.Address = $scope.data[0].Address;

            if ($scope.data[0].Landmark != undefined) {
                $scope.LandMark = $scope.data[0].Landmark;
            }
            $scope.PinCode = $scope.data[0].Pincode;
            $scope.Phone = $scope.data[0].Mobile;


            if ($scope.data[0].HowDoYouKnow != undefined) {
                $scope.HowDoYouKnow = $scope.data[0].HowDoYouKnow;
            }
            $scope.CollegeUniv = $scope.data[0].CollegeUniv;
            $scope.Qualification = $scope.data[0].Qualification;

            if ($scope.data[0].YearOfPassing != undefined) {
                $scope.YearOfPassing = $scope.data[0].YearOfPassing;
            }

            if ($scope.data[0].Comments != undefined) {
                $scope.Comments = $scope.data[0].Comments;
            }
            $scope.ImgOnlyOnEdit = true;
        })
    }

    // Document Init

    $scope.documentinit = function () {
        // $scope.docarray = [];

        $http.get('/User/InternshipDocumentData').then(function (response) {

            $scope.previewaadhar = true;
            $scope.hideaadhar = false;
            $scope.hideresume = false;
            $scope.previewresume = true;

            $scope.data = response.data;
            for (var i = 0; i < $scope.data.length; i++) {
                if ($scope.data[i].DoucmentNo == "No-Doc-Number-Resume") {
                    $scope.PdfDocumentId = $scope.data[i].DocumentId;
                    $scope.UserIdPdf = $scope.data[i].UserId;
                    $scope.fileResume = $scope.data[i].DocumentName;
                    $scope.DocumentNo = $scope.data[i].DoucmentNo;
                }
                else {

                    $scope.AadharDocumentId = $scope.data[i].DocumentId;
                    $scope.UserIdAadhar = $scope.data[i].UserId;
                    $scope.mdlInternApplyAadhar = $scope.data[i].DoucmentNo;
                    $scope.fileAadhar = $scope.data[i].DocumentName;
                }
            }
        })

    }

    $scope.changeInternImg = function (file) {
        $scope.FileName = file.$ngfName;
        $scope.fullfile = file;

        var files = $scope.fullfile;
        var title = files.$ngfName;
        var reader = new FileReader();
        if (files) {
            reader.readAsDataURL(files);
            var name = files.name;
        }

        reader.addEventListener("load", function () {
            // $scope.Preview = reader.result;
            $scope.Preview = reader.result;
            $scope.ImgOnlyOnEdit = false;
            $scope.PreviewImage = true;

        }, false);

        if ($scope.Preview !== undefined) {
            $scope.Preview = name;
            $scope.Title = $scope.Preview;
        }
    }


    // Upload Aadhar for Inter Applied Student/Professional By Atul on 21/09/2020
    $scope.uploadAadhaarForInterns = function (file) {
        $scope.previewaadhar = false;
        $scope.hideaadhar = true;
        $scope.aadhaarFileName = file.$ngfName;
        $scope.aadhaarFullfile = file;
    }

    //Upload Resume For Intern Applied Student/Professional By Atul on 21/09/2020
    $scope.uploadResumeForInterns = function (file) {
        $scope.hideresume = true;
        $scope.previewresume = false;
        $scope.resumeFileName = file.name;
        $scope.resumeFullfile = file;
    }


    // Update Internship Profile

    $scope.UpdateInternProfile = function () {
        $scope.InternProfile = {};
        $scope.Id = $scope.data[0].InterApllyId;
        $scope.InternProfile.Name = $scope.Name;
        $scope.InternProfile.DOB = moment($scope.DOB).format("YYYY-MM-DD");
        $scope.InternProfile.Email = $scope.Email;

        $scope.InternProfile.Twiter = $scope.Twitter;
        $scope.InternProfile.Instagram = $scope.Instagram;
        $scope.InternProfile.Facebook = $scope.Facebook;

        $scope.InternProfile.HowDoYouKnow = $scope.HowDoYouKnow;
        $scope.InternProfile.CollegeUniv = $scope.CollegeUniv;
        $scope.InternProfile.Qualification = $scope.Qualification;
        $scope.InternProfile.YearOfPassing = $scope.YearOfPassing;
        $scope.InternProfile.Comments = $scope.Comments;
        $scope.InternProfile.InternshipId = $scope.Id;

        $scope.InternContact = {};
        $scope.InternContact.State = $scope.State;
        $scope.InternContact.City = $scope.City;
        $scope.InternContact.PinCode = $scope.PinCode;
        $scope.InternContact.Address = $scope.Address;
        $scope.InternContact.LandMark = $scope.LandMark;
        $scope.InternContact.InternshipId = $scope.Id;

        $scope.arrInternApplyDoc = [];

        //if ($scope.resumeFileName !== undefined && $scope.resumeFileName != "") {
        //    $scope.arrInternApplyDoc.push({ DocumentId: $scope.PdfDocumentId, UserId: $scope.UserIdPdf, DocumentName: $scope.resumeFileName, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
        //}

        //if ($scope.mdlInternApplyAadhar !== undefined && $scope.mdlInternApplyAadhar != "") {
        //    $scope.arrInternApplyDoc.push({ DocumentId: $scope.AadharDocumentId, UserId: $scope.UserIdAadhar, DocumentName: $scope.aadhaarFileName, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
        //}

        if ($scope.resumeFileName !== undefined && $scope.resumeFileName != "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.PdfDocumentId, UserId: $scope.UserIdPdf, DocumentName: $scope.resumeFileName, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
        }
        else if ($scope.fileResume !== undefined && $scope.fileResume != "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.PdfDocumentId, UserId: $scope.UserIdPdf, DocumentName: $scope.fileResume, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
        }

        //$scope.mdlInternApplyAadhar !== undefined $scope.aadhaarFileName != undefined && $scope.aadhaarFileName != ""
        if ($scope.mdlInternApplyAadhar != undefined && $scope.mdlInternApplyAadhar != "" && $scope.fileAadhar.$ngfName === undefined || $scope.fileAadhar.$ngfName === "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.AadharDocumentId, UserId: $scope.UserIdAadhar, DocumentName: $scope.fileAadhar, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
        }

        //  $scope.mdlInternApplyAadhar !== undefined && $scope.mdlInternApplyAadhar !== "" || $scope.fileAadhar != undefined && $scope.fileAadhar != "" && $scope.fileAadhar.$ngfName != undefined && $scope.fileAadhar.$ngfName != ""
        else if ($scope.mdlInternApplyAadhar != undefined && $scope.mdlInternApplyAadhar != "" && $scope.aadhaarFileName !== undefined || $scope.aadhaarFileName !== "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.AadharDocumentId, UserId: $scope.UserIdAadhar, DocumentName: $scope.aadhaarFileName, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
        }


        Upload.upload({
            url: "/User/UpdateInternProfile",
            data: {
                postedfile: $scope.file,
                Username: $scope.Username,
                Phone: $scope.Phone,
                profile: $scope.InternProfile,
                InternshipId: $scope.Id,
                internContact: $scope.InternContact,
                lstDocuments: $scope.arrInternApplyDoc,
                resumePostedfile: $scope.resumeFullfile,
                aadhaarPostedfile: $scope.aadhaarFullfile
            }
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == "Updated") {
                alert("Data Updated Successfully");
                location.href = "/User/InternWelcome";
            }
            else if (response.data == "DateBirth") {
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 18',
                    icon: "warning",
                    dangerMode: "true"
                })
            }
        })
    }


    // Change Intern Password

    $scope.changeInternPassword = function () {

        $http.post("/User/ChangeInternStudentPassword", { strOldPassword: $scope.InternStudentCurrentPassword, strNewPassword: $scope.InternStudentNewPassword }).then(function (resChangInternPwd) {
            if (resChangInternPwd.data === "InternStudentOldPasswordNotMatch") {
                alert("Old password is wrong.");
            }
            else if (parseInt(resChangInternPwd.data) > 0) {
                $scope.AddActivity(parseInt(resChangInternPwd.data), "Intern Student", "Change Password", "Intern Student Change Password Msg", "Intern Student Change Password Desc");
                alert("Password successfully updated.");
                window.location.href = "/User/InternProfile";
                //  window.location.href = "/User/TutorLogin";
            }
            else {
                alert("Something went wrong. Please try again.");
            }
        })
    }


    $scope.changeAdminPassword = function () {

    }


    //$scope.changeInternPassword = function () {

    //    if ($scope.NewPassword === $scope.ReTypePassword) {

    //        $http.post('/User/ChangeInternsPasword', {
    //            Email: $scope.Email, CurrentPassword: $scope.CurrentPassword,
    //            NewPassword: $scope.NewPassword, ReTypePassword: $scope.ReTypePassword
    //        }).then(function (response) {
    //            $scope.data = response.data;

    //            if (response.data == "currpasswrong") {
    //                swal({
    //                    title: 'Error!',
    //                    text: 'Current Password is wrong',
    //                    icon: "warning",
    //                    dangerMode: "true"
    //                })

    //            }
    //            else if (response.data == "passupdated") {               
    //                alert("Password Changed Successfully");
    //                location.href = "/User/InternLogin";
    //            }
    //            else if (response.data == "notregistered") {
    //                swal({
    //                    title: 'Error!',
    //                    text: 'Intern not registered',
    //                    icon: "warning",
    //                    dangerMode: "true"
    //                })
    //            }
    //            else if (response.data == "currnewpasswrong") {
    //                swal({
    //                    title: 'Error!',
    //                    text: 'New Password and Retype Password must match',
    //                    icon: "warning",
    //                    dangerMode: "true"
    //                })
    //            }
    //            else {
    //                swal({
    //                    title: 'Error!',
    //                    text: 'Error while changing passsword Please try again',
    //                    icon: "warning",
    //                    dangerMode: "true"
    //                })
    //            }
    //        })
    //    }
    //    else {
    //        swal({
    //            title: 'Error!',
    //            text: 'New Password and Retype Password must match',
    //            icon: "warning",
    //            dangerMode: "true"
    //        })
    //    }
    //}


    // Validate Itnern DOB


    $scope.validateInternDOB = function () {

        if ($scope.DOB !== undefined) {
            var a = moment();
            var b = moment($scope.DOB);
            var yearsage = a.diff(b, 'years');
            var age = yearsage;
            if (age < 18) {
                swal({
                    title: 'Error!',
                    text: 'Age can not be smaller than 18',
                    icon: "warning",
                    dangerMode: "true"
                })

            }
            else {

            }
        }
    }


    // Internship Details Init:

    $scope.internshipdetailsinit = function () {

        $http.get('/User/GetInternshipDataBasedOnInternApplyId').then(function (response) {
            $scope.data = response.data;

            $scope.data2 = JSON.parse($scope.data);

            $scope.internshiparrayparse = [];
            $scope.perksarrayparse = [];

            $scope.internshiparrayparse = $scope.data2["internshipdetail"];
            $scope.perksarrayparse = $scope.data2["perks"];

            $scope.InternshipDetailsArray = [];

            //if ($scope.data2["internshipdetail"][0].InternshipName != "" && $scope.data2["internshipdetail"][0].InternshipName != undefined) {
            //    for (var i = 0; i < $scope.internshiparrayparse[i].length; i++) {

            //       $scope.InternshipDetailsArray.push({
            //          InternshipName: $scope.internshiparrayparse[i].InternshipName, DurationName: $scope.internshiparrayparse[i].DurationName,
            //            ValidFrom: $scope.internshiparrayparse[i].ValidFrom, ValidTo: $scope.internshiparrayparse[i].ValidTo
            //        })
            //    }
            //}

            // Add InternApplyId and UserType for Communication by Dilshad A. on 25 Sept 2020
            $scope.arrIdUserType = [];
            $scope.arrIdUserType.push({ Id: $scope.internshiparrayparse[0].InterApllyId, UserType: "All-Intern-Students" });
            sessionStorage.setItem("InternIdUserNameForCommu", JSON.stringify($scope.arrIdUserType));

            $scope.perksdataArray = [];


            //if ($scope.data2["perks"][0].PerkName != "" && $scope.data2["perks"][0].PerkName != undefined) {
            //    for (var i = 0; i < $scope.perksarrayparse[i].length; i++) {
            //        $scope.perksdataArray.push({
            //            PerkName: $scope.perksarrayparse[i].PerkName
            //        })
            //    }
            //}
        })

    }



    // Show Added Category StudentCategory by admin show 

    $scope.studentcategoryinit = function () {

        $http.get('/Admin/GetStudentCategories').then(function (response) {
            $scope.data = response.data;

            $scope.ShowCategory = new NgTableParams({}, { dataset: $scope.data });
        })
    }


    // Student Category Add

    $scope.saveStudentCategory = function () {

        if (sessionStorage.StudentCategoryId !== undefined && sessionStorage.StudentCategoryId !== undefined) {
            $scope.data3 = JSON.parse(sessionStorage.StudentCategoryId);

            if ($scope.data3 !== undefined && $scope.data3 != '') {
                $scope.StudentCategoryUpdate = {};
                $scope.StudentCategoryUpdate.CategoryName = $scope.CategoryName;
                $scope.StudentCategoryUpdate.Description = $scope.Description;
                $scope.StudentCategoryUpdate.IsPublished = $scope.mdlStudentCategoryIsPublished;
                $scope.StudentCategoryUpdate.StudentCategoryId = $scope.data3.StudentCategoryId;

            }

            $http.post('/Admin/UpdateStudentCategory', { updatecategory: $scope.StudentCategoryUpdate }).then(function (response) {
                $scope.data = response.data;

                if ($scope.data == true) {
                    sessionStorage.clear();
                    alert("Student Category updated successfully");
                    location.href = "/Admin/StudentCategory";
                }

                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("Student Category already exists");
                }
                else {
                    alert("Error while Updating Student Category");
                }

            })
        }

        else {

            $scope.AddStudentCategory = {};
            $scope.AddStudentCategory.CategoryName = $scope.CategoryName;
            $scope.AddStudentCategory.Description = $scope.Description;
            $scope.AddStudentCategory.IsPublished = $scope.mdlStudentCategoryIsPublished;

            $http.post('/Admin/SaveStudentCategory', { addcategory: $scope.AddStudentCategory }).then(function (response) {
                $scope.data = response.data;

                if ($scope.data == true) {
                    alert("Student Category created successfully");
                    location.href = "/Admin/StudentCategory";
                }
                else if (response.data == false) {
                    alert("Student Category already exists");
                }
                else if (response.data == "empty") {
                    swal({
                        title: 'Error',
                        text: 'Student Category is mandatory',
                        icon: "warning",
                        dangerMode: "true"
                    });

                }

                else {
                    alert("Error! Problem while creating Student Category");
                }
            })
        }
    }


    // Delete Student Category

    $scope.deleteStudentCategory = function (Id) {
        $scope.Id = Id;
    }


    // Delete Category 

    $scope.DeleteStudentCategory = function (Id) {
        $http.post('/Admin/DeleteStudentCategory', {
            Id: $scope.Id
        }).then(function (response) {
            $scope.data = response.data;

            if (parseInt(response.data) > 0) {
                alert("Student Category Deleted Successfully");
                window.location.href = "/Admin/StudentCategory";
            }

            else {
                alert("Error while deleting Student Category");
            }

        })
    }

    // Edit Category

    $scope.editStudentCategory = function (Id) {
        $scope.Id = Id;
        sessionStorage.StudentCategoryId = JSON.stringify($scope.Id);

        if (sessionStorage.StudentCategoryId !== undefined && sessionStorage.StudentCategoryId != '') {
            $scope.data2 = JSON.parse(sessionStorage.StudentCategoryId);
        }

        if ($scope.data2 !== undefined && $scope.data2 != '') {
            $scope.Description = $scope.data2.Description;
            $scope.CategoryName = $scope.data2.CategoryName;
            $scope.mdlStudentCategoryIsPublished = $scope.data2.IsPublished;
            $scope.StudentCategoryId = $scope.data2.StudentCategoryId;
        }
    }


    // Add Course Content

    $scope.addcoursecontentinit = function () {

        $scope.hideCourse = true;
        $scope.hideInternship = true;
        $scope.hideCourseContentHeading = true;
        $scope.hidesubheadingchoice = true;
        $scope.hidesubheadingcontent = true;
        $scope.hideDescription = true;
        $scope.hideDescriptionTable = true;
        $scope.hidePrerequisities = true;
        $scope.hideShortDescription = true;
        $scope.btnhideprequisities = true;
        $scope.btnhidebtnhidedescription = true;
        $scope.hidePrerequisitiesTable = true;

        if (sessionStorage.EditCourseData != '' && sessionStorage.EditCourseData != undefined) {
            $scope.hideUpdatebtn = false;
            $scope.Insertbtn = true;
        }
        else {
            $scope.hideUpdatebtn = true;
            $scope.Insertbtn = true;
        }

        $http.get('/Course/GetAddedCourses').then(function (response) {
            $scope.CoursesData = response.data;
        })

        $http.get('/Internship/GetInternship').then(function (response) {
            $scope.InternshipsData = response.data;
        })
    }




    $scope.ChangeCourseType = function () {

        if ($scope.CourseType == "Course") {
            $scope.hideCourse = false;
            $scope.hideInternship = true;
            $scope.hideCourseContentHeading = true;
            $scope.hidesubheadingcontent = true;
            $scope.hideDescription = true;
            $scope.Insertbtn = true;
        }
        else if ($scope.CourseType == "Intern") {
            $scope.hideInternship = false;
            $scope.hideCourse = true;
            $scope.hideCourseContentHeading = true;
            $scope.hidesubheadingcontent = true;
            $scope.hideDescription = true;
            $scope.Insertbtn = true;
        }

    }

    $scope.changeCourse = function () {

        if ($scope.Course != undefined && $scope.Course != "") {
            $scope.hideCourseContentHeading = false;
            $scope.hidesubheadingchoice = false;
            $scope.hideDescription = true;
            $scope.Insertbtn = true;

            if (sessionStorage.EditCourseData == '' || sessionStorage.EditCourseData == undefined) {
                $scope.Id = $scope.Course;

                $scope.hidecourseheading = false;

                $http.get(`/Admin/GetCourseHeading?CourseId=${$scope.Id}`).then(function (response) {
                    $scope.CourseHeadingDesc = response.data;
                    $scope.CourseHeadingDescription = JSON.parse($scope.CourseHeadingDesc);

                    $scope.CourseHeadingdata = [];
                    $scope.Descriptiondata = [];

                    $scope.CourseHeadingdata = $scope.CourseHeadingDescription["coursecont"];
                    $scope.Descriptiondata = $scope.CourseHeadingDescription["description"];

                    $scope.CourseHeadingArray = [];
                    $scope.DescriptionArray = [];

                    // Added by Dilshad A. on 31 Oct 2020
                    if ($scope.CourseHeadingdata.length > 0) {
                        $scope.ShortDescription = $scope.CourseHeadingdata[0].ShortDescription;
                    }
                    // END Added by Dilshad A. on 31 Oct 2020

                    for (var i = 0; i < $scope.CourseHeadingdata.length; i++) {
                        $scope.CourseHeadingArray.push({ ContentId: $scope.CourseHeadingdata[i].ContentId, CourseContentHeading: $scope.CourseHeadingdata[i].CourseContentHeading });
                    }

                    for (var i = 0; i < $scope.Descriptiondata.length; i++) {
                        $scope.DescriptionArray.push({ ContentId: $scope.Descriptiondata[i].ContentId, DescriptionId: $scope.Descriptiondata[i].DescriptionId, Description: $scope.Descriptiondata[i].Description, IsPublished: $scope.Descriptiondata[i].IsPublished });
                    }

                })
            }

        }
        else {
            $scope.hideCourseContentHeading = true;
            $scope.hidesubheadingchoice = true;
            $scope.hidesubheadingcontent = true;
            $scope.Insertbtn = true;
            $scope.hidecourseheading = true;
        }
    }

    $scope.changeInternship = function () {
        if ($scope.Internship != undefined && $scope.Internship != "") {
            $scope.hideCourseContentHeading = false;
            $scope.hidesubheadingchoice = true;
            $scope.hidesubheading = true;
            $scope.hidesubheadingcontent = true;
            $scope.hidesubheadingchoice = false;
            $scope.hideDescription = true;
            $scope.Insertbtn = true;

            if (sessionStorage.EditCourseData == '' || sessionStorage.EditCourseData == undefined) {
                $scope.Id = $scope.Internship;
                $scope.hideinternheading = false;
                $http.get(`/Admin/GetInternHeading?InternId=${$scope.Id}`).then(function (response) {
                    $scope.InternHeadingDesc = response.data;
                    $scope.InternHeadingDescription = JSON.parse($scope.InternHeadingDesc);

                    $scope.InternHeadingdata = [];
                    $scope.InternDescriptiondata = [];

                    $scope.InternHeadingdata = $scope.InternHeadingDescription["Interncont"];
                    $scope.InternDescriptiondata = $scope.InternHeadingDescription["interndescription"];

                    $scope.InternHeadingArray = [];
                    $scope.InternDescriptionArray = [];

                    //Added on 10 Nov 2020 BY Dilshad A.
                    if ($scope.InternHeadingdata.length > 0) {
                        $scope.ShortDescription = $scope.InternHeadingdata[0].ShortDescription;
                    }
                    // END Added on 10 Nov 2020 BY Dilshad A.s

                    for (var i = 0; i < $scope.InternHeadingdata.length; i++) {
                        $scope.InternHeadingArray.push({ ContentId: $scope.InternHeadingdata[i].ContentId, InternContentHeading: $scope.InternHeadingdata[i].CourseContentHeading });
                    }

                    for (var i = 0; i < $scope.InternDescriptiondata.length; i++) {
                        $scope.InternDescriptionArray.push({
                            ContentId: $scope.InternDescriptiondata[i].ContentId, DescriptionId: $scope.InternDescriptiondata[i].DescriptionId,
                            Description: $scope.InternDescriptiondata[i].Description, IsPublished: $scope.InternDescriptiondata[i].IsPublished
                        });
                    }

                })

            }


        }
        else {
            $scope.hideCourseContentHeading = true;
            $scope.hidesubheadingchoice = false;
            $scope.hidesubheadingcontent = true;
            $scope.Insertbtn = true;
            $scope.hideinternheading = true;
        }

    }

    $scope.ChangeSubHeadingChoice = function () {


        if ($scope.SubHeadingChoice != undefined && $scope.SubHeadingChoice != "" && $scope.SubHeadingChoice == "Yes") {
            $scope.hidesubheadingcontent = false;
            $scope.hidesubheadingchoice = true;
            $scope.hideDescription = false;
            $scope.hideDescriptionTable = false;
            $scope.hidePrerequisitiesTable = false;
            $scope.hideShortDescription = false;
            $scope.hidePrerequisities = false;
            $scope.btnhideprequisities = false;
            $scope.btnhidebtnhidedescription = false;

            // $scope.Insertbtn = true;

            if (sessionStorage.EditCourseData != '' && sessionStorage.EditCourseData != undefined) {
                $scope.hideUpdatebtn = false;
                $scope.Insertbtn = true;
            }
            else {
                $scope.hideUpdatebtn = true;
                $scope.Insertbtn = false;
            }
        }
        else {
            $scope.hideShortDescription = false;
            $scope.hidesubheadingcontent = true;
            $scope.hideDescription = false;
            $scope.hideDescriptionTable = false;
            $scope.hidePrerequisitiesTable = false;
            $scope.hidePrerequisities = false;
            $scope.btnhideprequisities = false;
            $scope.btnhidebtnhidedescription = false;

            if (sessionStorage.EditCourseData != '' && sessionStorage.EditCourseData != undefined) {
                $scope.hideUpdatebtn = false;
                $scope.Insertbtn = true;
            }
            else {
                $scope.hideUpdatebtn = true;
                $scope.Insertbtn = false;
            }
        }
    }


    // Add Description

    $scope.arrCourseContentDescription = [];
    $scope.AddMultipleDescription = function () {
        if ($scope.Description !== "" && $scope.Description !== undefined) {
            $scope.arrCourseContentDescription.push({ IsPublished: $scope.mdlStudentCategoryIsPublished, Description: $scope.Description });

        }
        else {
            alert("Description is mandatory.");
        }
        $scope.mdlStudentCategoryIsPublished = "";
        $scope.Description = "";
    }


    // Add Multiple Prerequisities

    $scope.arrCoursePrerequisities = [];
    $scope.AddMultiplePrequisities = function () {
        if ($scope.PrerequisityPoint !== "" && $scope.PrerequisityPoint !== undefined) {
            $scope.arrCoursePrerequisities.push({ IsPublished: $scope.mdlPrerequisitiesIsPublished, PrerequisitePoints: $scope.PrerequisityPoint });

        }
        else {
            alert("Prerequisites are mandatory.");
        }
        $scope.mdlPrerequisitiesIsPublished = "";
        $scope.PrerequisityPoint = "";
    }


    // Delete Prerequisities

    $scope.deletePrerequisityArray = [];

    $scope.DeletePrequisities = function (index) {

        var data2 = $scope.arrCoursePrerequisities[index];
        $scope.deletePrerequisityArray.push({ PrerequisiteId: data2.PrerequisiteId, PrerequisitePoints: data2.PrerequisitePoints, IsPublished: data2.IsPublished });
        $scope.arrCoursePrerequisities.splice(index, 1);
    }


    // Delete Description

    $scope.deleteDescriptionArray = [];

    $scope.DeleteDescription = function (index) {

        var data2 = $scope.arrCourseContentDescription[index];
        $scope.deleteDescriptionArray.push({ DescriptionId: data2.DescriptionId, Description: data2.Description, IsPublished: data2.IsPublished });
        $scope.arrCourseContentDescription.splice(index, 1);
    }


    // View Course Content

    $scope.ViewCourseContent = function () {

        $http.get('/Admin/ViewCourseContent').then(function (response) {
            $scope.data = response.data;
            $scope.CourseContentData = new NgTableParams({}, { dataset: $scope.data });
        })
    }


    // Insert 

    $scope.AddCourseContent = function () {


        $scope.CourseContent = {};

        $scope.CourseContent.CourseType = $scope.CourseType;

        //$scope.Course;
        //$scope.Internship;

        $scope.CourseContent.CourseContentHeading = $scope.CourseContentHeading;



        if ($scope.CourseType == "Course") {
            $scope.CourseContent.CourseId = $scope.Course;
        }
        else if ($scope.CourseType == "Intern") {
            $scope.CourseContent.InternCourseId = $scope.Internship;
        }

        if ($scope.SubHeadingChoice != "" && $scope.SubHeadingChoice !== undefined && $scope.SubHeadingChoice == "Yes") {

            $scope.CourseContent.IsSubHeading = true;
        }
        else if ($scope.SubHeadingChoice != "" && $scope.SubHeadingChoice !== undefined && $scope.SubHeadingChoice == "No") {
            $scope.CourseContent.IsSubHeading = false;
        }

        if ($scope.SubHeading != "" && $scope.SubHeading != undefined) {
            $scope.CourseContent.SubHeading = $scope.SubHeading;
        }

        if ($scope.ShortDescription != "" && $scope.ShortDescription != undefined) {
            $scope.CourseContent.ShortDescription = $scope.ShortDescription;
        }


        $scope.CourseContent.IsPublished = $scope.IsPublished;


        $http.post('/Admin/AddCourseContentDescription', {
            coursecontent: $scope.CourseContent, ContentDescriptionArray: $scope.arrCourseContentDescription,
            arrCoursePrerequisities: $scope.arrCoursePrerequisities
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == true) {
                alert("Course Content Description Inserted Successfully");
                location.href = "/Admin/ShowCourseContent";
            }

        })


    }


    // Edit Course Content

    $scope.EditCourseContent = function (ContentId, CourseType) {

        $scope.ContentId = ContentId;
        $scope.CourseType = CourseType.trim();


        $http.get(`/Admin/EditCourseContent?id=${$scope.ContentId}&userType= ${$scope.CourseType}`).then(function (response) {
            $scope.data = response.data;
            sessionStorage.EditCourseData = $scope.data;
            location.href = "/Admin/AddCourseContent";


        });

    }


    // Update Course Content Init

    $scope.updatecoursecontentinit = function () {

        if (sessionStorage.EditCourseData != '' && sessionStorage.EditCourseData != undefined) {
            $http.get('/Course/GetAddedCourses').then(function (response) {
                $scope.CoursesData = response.data;
            })

            $http.get('/Internship/GetInternship').then(function (response) {
                $scope.InternshipsData = response.data;
            })

            $scope.data2 = JSON.parse(sessionStorage.EditCourseData);

            $scope.hideShortDescription = false;
            $scope.hidePrerequisitiesTable = false;
            $scope.hidePrerequisities = false;
            $scope.btnhideprequisities = false;
            $scope.btnhidebtnhidedescription = false;

            if ($scope.data2["coursecontent"][0] != '' && $scope.data2["coursecontent"][0] != undefined) {

                $scope.hideUpdatebtn = true;
                $scope.Insertbtn = true;
                $scope.IsPublished = $scope.data2["coursecontent"][0].IsPublished;
                $scope.CourseType = $scope.data2["coursecontent"][0].CourseType;
                $scope.CourseContentHeading = $scope.data2["coursecontent"][0].CourseContentHeading;
                $scope.Id = $scope.data2["coursecontent"][0].ContentId;

                if ($scope.data2["coursecontent"][0].ShortDescription != undefined && $scope.data2["coursecontent"][0].ShortDescription != '') {
                    $scope.hideShortDescription = false;
                    $scope.ShortDescription = $scope.data2["coursecontent"][0].ShortDescription;
                }


                if ($scope.CourseType == "Course") {
                    $scope.hideCourse = false;
                    $scope.hideInternship = true;
                    $scope.Course = $scope.data2["coursecontent"][0].CourseId;
                }
                else if ($scope.CourseType == "Intern") {
                    $scope.hideInternship = false;
                    $scope.hideCourse = true;
                    $scope.Internship = $scope.data2["coursecontent"][0].InternCourseId;
                }


                $scope.CourseContentHeading = $scope.data2["coursecontent"][0].CourseContentHeading;

                if ($scope.CourseContentHeading != '' && $scope.CourseContentHeading != undefined) {
                    $scope.hideCourseContentHeading = false;
                }

                if ($scope.data2["coursecontent"][0].IsSubHeading == true) {
                    $scope.hidesubheadingchoice = false;
                    $scope.SubHeadingChoice = "Yes";

                    $scope.hidesubheadingcontent = false;
                    $scope.SubHeading = $scope.data2["coursecontent"][0].SubHeading;
                    $scope.hideDescription = false;
                    $scope.hideUpdatebtn = false;
                    $scope.Insertbtn = true;
                    $scope.hideDescriptionTable = false;

                }
                else if ($scope.data2["coursecontent"][0].IsSubHeading == false) {
                    $scope.hidesubheadingchoice = false;
                    $scope.SubHeadingChoice = "No";
                    $scope.hidesubheadingcontent = true;
                    $scope.hideDescription = false;
                    $scope.hideUpdatebtn = false;
                    $scope.Insertbtn = true;
                    $scope.hideDescriptionTable = false;
                }
            }

            $scope.arrCourseContentDescription = [];
            $scope.data3 = [];

            $scope.data3 = $scope.data2["description"];

            if ($scope.data2["description"] !== undefined && $scope.data2["description"] != '') {

                for (var i = 0; i < $scope.data3.length; i++) {
                    $scope.arrCourseContentDescription.push({ DescriptionId: $scope.data3[i].DescriptionId, Description: $scope.data3[i].Description, IsPublished: $scope.data3[i].IsPublished });
                }

            }


            $scope.arrCoursePrerequisities = [];
            $scope.data4 = [];
            $scope.data4 = $scope.data2["prerequisities"];

            if ($scope.data2["prerequisities"] !== undefined && $scope.data2["prerequisities"] != '') {

                for (var i = 0; i < $scope.data4.length; i++) {
                    $scope.arrCoursePrerequisities.push({ PrerequisiteId: $scope.data4[i].PrerequisiteId, PrerequisitePoints: $scope.data4[i].PrerequisitePoints, IsPublished: $scope.data4[i].IsPublished });
                }

            }

        }


    }

    // Update Course Content


    $scope.UpdateCourseContentDescription = function () {

        $scope.UpdateCourseContents = {};
        $scope.UpdateCourseContents.ContentId = $scope.Id;
        $scope.UpdateCourseContents.IsPublished = $scope.IsPublished;
        $scope.UpdateCourseContents.CourseType = $scope.CourseType;
        $scope.UpdateCourseContents.CourseContentHeading = $scope.CourseContentHeading;


        if ($scope.CourseType == "Course") {
            $scope.UpdateCourseContents.CourseId = $scope.Course;
        }
        else if ($scope.CourseType == "Intern") {
            $scope.UpdateCourseContents.InternCourseId = $scope.Internship;
        }

        if ($scope.SubHeadingChoice != "" && $scope.SubHeadingChoice !== undefined && $scope.SubHeadingChoice == "Yes") {

            $scope.UpdateCourseContents.IsSubHeading = true;
        }
        else if ($scope.SubHeadingChoice != "" && $scope.SubHeadingChoice !== undefined && $scope.SubHeadingChoice == "No") {
            $scope.UpdateCourseContents.IsSubHeading = false;
        }

        if ($scope.SubHeading != "" && $scope.SubHeading != undefined) {
            $scope.UpdateCourseContents.SubHeading = $scope.SubHeading;
        }

        if ($scope.ShortDescription != "" && $scope.ShortDescription != undefined) {
            $scope.UpdateCourseContents.ShortDescription = $scope.ShortDescription;
        }


        $http.post('/Admin/UpdateCourseContentDescription', {
            updatecoursecontent: $scope.UpdateCourseContents, updatecoursearr: $scope.arrCourseContentDescription, deleteDescriptionArray: $scope.deleteDescriptionArray,
            arrCoursePrerequisities: $scope.arrCoursePrerequisities, deletePrerequisityArray: $scope.deletePrerequisityArray
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                sessionStorage.clear();
                alert("Course Content Description Updated Successfully");
                location.href = "/Admin/ShowCourseContent";
            }

        })
    };




    // Get Course Tutors Data (Id,Name)

    $scope.GetTutorsData = function () {
        //GetTutorsData

        $scope.hideCourse = true;
        $scope.hideInternship = true;
        $scope.hideBrandCourse = true;

        $http.get("/Admin/GetCourseTutorsData").then(function (response) {
            $scope.TutorsBind = response.data;
        })

        $http.get("/Admin/GetInternTutorsData").then(function (response) {
            $scope.InternTutorsBind = response.data;
        })

        $http.get("/User/GetCourses").then(function (response) {
            $scope.CoursesForTutorBind = response.data;
        })

        $http.get("/Internship/GetAddedInternships").then(function (response) {
            $scope.InternshipForTutorBind = response.data;
        })

        $http.get("Tutor/CoursesForBrandTutors").then(function (response) {
            $scope.CourseForBrandTutorBind = response.data;
        })

    }


    $scope.GetInternshipsData = function () {
        $http.get('/Internship/GetInternship').then(function (response) {
            $scope.InternshipBind = response.data;
        })
    }


    // Get Tutor details based on Course Id 

    $scope.GetTutorDetailsByCourseId = function (id) {

        if ($scope.mdlCategoryTypeForTutor == "Course") {
            $http.post("/Admin/GetTutorsByCourseId", { intCourseId: id }).then(function (response) {
                $scope.CourseTutors = response.data;

                $scope.TutorListByCourse = new NgTableParams({}, { dataset: $scope.CourseTutors });
                $scope.shListOfCourseTutor = true;

            })
        }
        else if ($scope.mdlCategoryTypeForTutor == "Brand") {

            $http.post("/Admin/GetTutorsByCourseIdForBrand", { intBrandId: id }).then(function (response) {
                $scope.BrandTutorsAd = response.data;

                $scope.BrandTutorListByCourse = new NgTableParams({}, { dataset: $scope.BrandTutorsAd });
                $scope.shListOfBrandTutor = true;

            })
        }

    }

    // Get Tutor Details based on InternshipId 

    $scope.GetInternDetailsByInternshipId = function (id) {
        $http.post("/Admin/GetTutorsByInternshipId", { intInternshipId: id }).then(function (response) {
            $scope.InternTutors = response.data;

            $scope.TutorListByInternship = new NgTableParams({}, { dataset: $scope.InternTutors });
            $scope.shListOfInternTutor = true;

        })
    }

    //Select Multiple Course Tutor For Admin To Course Tutor Communication By Atul Sh. on 12 Nov 2020
    $scope.arrAdminToCourseTutor = [];
    $scope.SelectMultipleCourseTutorForCommunication = function () {
        var data = $scope.CourseTutors;
        $scope.arrAdminToCourseTutors = [];
        for (var i = 0; i < $scope.CourseTutors.length; i++) {
            if ($scope.CourseTutors[i].chkAdminToCourseTutor == true) {
                $scope.arrAdminToCourseTutors.push({
                    Name: data[i].TutorName, Email: data[i].TutorEmail, TutorId: data[i].TutorId, CourseId: data[i].CourseId,
                    Id: data[i].CourseTutorId
                });
            }
        }
    }


    //Select Multiple Intern Tutor For Admin To Intern Tutor Communication By Atul Sh. on 12 Nov 2020

    $scope.arrAdminToInternTutor = [];
    $scope.SelectMultipleInternTutorCommunicatio = function () {
        var data = $scope.InternTutors;
        $scope.arrAdminToInternTutors = [];
        for (var i = 0; i < $scope.InternTutors.length; i++) {
            if ($scope.InternTutors[i].chkAdminToInternTutor == true) {
                $scope.arrAdminToInternTutors.push({
                    Name: data[i].TutorName, Email: data[i].TutorEmail, TutorId: data[i].TutorId, InternshipId: data[i].InternshipId,
                    Id: data[i].InternTutorId
                });
            }
        }
    }

    // Admin To Brand Tutor Communication Select Single Or Multiple

    $scope.AdminToBrandTutorSelectMultipleCommunication = function () {
        var data = $scope.BrandTutorsAd;
        $scope.arrAdminToBrandTutorsCoom = [];
        for (var i = 0; i < $scope.BrandTutorsAd.length; i++) {
            if ($scope.BrandTutorsAd[i].chkAdminToBrandTutorcheck == true) {
                $scope.arrAdminToBrandTutorsCoom.push({
                    Name: data[i].TutorName, Email: data[i].TutorEmail, TutorId: data[i].TutorId, CourseId: data[i].CourseId,
                    Id: data[i].StructureId
                });
            }
        }
    }


    //select tut.TutorId, TutorName, TutorEmail, intntut.InternTutorId, intntut.TutorId, intntut.InternshipId from SITSPL_tblTutor as tut
    //INNER JOIN tblInternTutor as intntut
    //on tut.TutorId = intntut.TutorId
    //INNER JOIN SITSPL_tblInternshipStructure as intstr
    //on intstr.InternshipId = intntut.InternshipId;

    $scope.getTutors = function () {

        if ($scope.mdlCategoryTypeForTutor == "Intern") {
            $scope.hideInternship = false;
            $scope.hideCourse = true;
            $scope.shListOfCourseTutor = false;
            $scope.shListOfInternTutor = true;
        }
        else if ($scope.mdlCategoryTypeForTutor == "Course") {
            $scope.hideInternship = true;
            $scope.hideCourse = false;
            $scope.shListOfCourseTutor = true;
            $scope.shListOfInternTutor = false;
        }

        else if ($scope.mdlCategoryTypeForTutor == "Brand") {

            $scope.hideInternship = true;
            $scope.hideCourse = false;
           // $scope.hideCourse = true;
            $scope.shListOfCourseTutor = false;
            $scope.shListOfInternTutor = false;
            $scope.shListOfBrandTutor = true;


            //$scope.shListOfCourseTutor = false;
            //$scope.shListOfInternTutor = false;


            //$scope.hideInternship = true;
            //$scope.hideCourse = false;
            //$scope.shListOfCourseTutor = true;
            //$scope.shListOfInternTutor = false;
        }

        

    }

    $scope.GetTutorId = function (TutorId) {
        $scope.TutId = TutorId;
    }


    $scope.GetTutorForInternship = function (Id) {

        $scope.Internship = Id;
        $scope.TutorId = $scope.TutId;

        $scope.shListOfInternTutor = true;

        $http.post('/Admin/GetTutorForInternship', { InternId: $scope.Internship, TutorId: $scope.TutorId }).then(function (response) {
            $scope.TutorForInternship = response.data;

            $scope.TutorForInternshipData = new NgTableParams({}, { dataset: $scope.TutorForInternship });
        })

    }


    //Select Multiple Intern Tut For COmmunication By Dilshad A. on 22 Sept 2020
    $scope.arrInternTutor = [];
    $scope.SelectMultipleInternTutorForCommunication = function () {
        var data = $scope.TutorForInternship;
        $scope.arrTutors = [];
        for (var i = 0; i < $scope.TutorForInternship.length; i++) {
            if ($scope.TutorForInternship[i].chkInternTutor == true) {
                $scope.arrInternTutor.push({
                    Name: data[i].TutorName, Email: data[i].TutorEmail, TutorId: data[i].TutorId, InternshipId: data[i].InternshipId,
                    Id: data[i].InternTutorId
                });
            }
        }
    }


    $scope.AddMessageForTutorCommunication = function () {

        if ($scope.chkAllInternTutors == undefined || $scope.chkAllInternTutors == "") {
            $scope.chkAllInternTutors = false;
        }

        if ($scope.chkAllCourseTutors == undefined || $scope.chkAllCourseTutors == "") {
            $scope.chkAllCourseTutors = false;
        }

        if ($scope.chkAllBrandTutors == undefined || $scope.chkAllBrandTutors == "") {
            $scope.chkAllBrandTutors = false;
        }

        



        $http.post('/Admin/AddMessageForTutorCommunication',
            {
                CourseId: $scope.mdlCourse, InternshipId: $scope.mdlInternship, lstInternTutor: $scope.arrInternTutor, isAllInterns: $scope.chkAllInternTutors,
                isAllCourses: $scope.chkAllCourseTutors, CategoryType: $scope.mdlCategoryTypeForTutor, strMsgTutor: $scope.mdlMsgOnlyTutor,
                lstCourseTutor: $scope.arrAdminToCourseTutors, lstInternsTutor: $scope.arrAdminToInternTutors,
                lstBrandTutor: $scope.arrAdminToBrandTutorsCoom, isAllBrandTutor: $scope.chkAllBrandTutors
            }).then(function (response) {

                $scope.MessageForTutorData = response.data;

                if (response.data > 0) {
                    alert("Message successfully sent.");
                    window.location.href = "/Admin/Communication";
                }
                else {
                    console.log(response.data);
                    alert("Failed. Something went wrong.");
                }
            })

    }


    // Student Communication

    $scope.GetCommunicationByStudentId = function (UserTye, BrandType) {

        var d = sessionStorage.getItem("StudentIdUserNameForCommu");
        var e = sessionStorage.getItem("BrandStudentIdUserType");

        if (UserTye === 'All-Students-Course' && d != undefined && d != '') {
            var d = sessionStorage.getItem("StudentIdUserNameForCommu");
            d = JSON.parse(d);

            $scope.hideInternStudentCommunication = false;
            $scope.hideBrandStudentCommunication = true;
            $scope.brandtutorcomm = true;

            $http.post("/User/GetCommunicationById", { intId: d[0].Id, strUserType: d[0].UserType }).then(function (res) {
                //$scope.CommByUserId = res.data.data;
                //$scope.CommForAll = res.data.msgAll;

                $scope.CommunicationByUserId = res.data.data;
                $scope.InternCommunicationById = new NgTableParams({}, { dataset: $scope.CommunicationByUserId });
                //      $scope.CommForAll = res.data.msgAll;

            });
        }

        // Brand Student

        if (BrandType === 'All-Brand-Students' && e != undefined && e != '') {
            var e = sessionStorage.getItem("BrandStudentIdUserType");
            e = JSON.parse(e);

            $scope.hideInternStudentCommunication = true;
            $scope.hideBrandStudentCommunication = false;
            $scope.brandtutorcomm = true;

            $http.post("/User/GetCommunicationById", { intId: e[0].Id, strUserType: e[0].UserType }).then(function (res) {
                //$scope.CommByUserId = res.data.data;
                //$scope.CommForAll = res.data.msgAll;

                $scope.BrandStudentMsgByUserId = res.data.data;
                $scope.BrandStudentMsg = new NgTableParams({}, { dataset: $scope.BrandStudentMsgByUserId });
                //   $scope.BrandStudentMsgForAll = res.data.msgAll;

            });
        }
    };





    // Add Communication Message To All

    $scope.AddCommunicationMessageForAll = function () {

        if ($scope.chkAllInternTutors == undefined || $scope.chkAllInternTutors == "") {
            $scope.chkAllInternTutors = false;
        }

        if ($scope.chkAllCourseTutors == undefined || $scope.chkAllCourseTutors == "") {
            $scope.chkAllCourseTutors = false;
        }

        if ($scope.chkAllCourseStudents == undefined || $scope.chkAllCourseStudents == "") {
            $scope.chkAllCourseStudents = false;
        }

        if ($scope.chkAllInternStudents == undefined || $scope.chkAllInternStudents == "") {
            $scope.chkAllInternStudents = false;
        }


        $http.post('/Admin/AddCommunicationMessageForAll', {
            lstInternTutor: $scope.chkAllInternTutors, lstCourseTutor: $scope.chkAllCourseTutors,
            lstCourseStudent: $scope.chkAllCourseStudents, lstInternStudent: $scope.chkAllInternStudents, strMsgAll: $scope.mdlMsgAll
        }).then(function (response) {
            $scope.AddCommunicationMessageForAll = response.data;

            if (response.data > 0) {
                alert("Message successfully sent.");
                window.location.href = "/Admin/Communication";
            }
            else {
                console.log(response.data);
                alert("Failed. Something went wrong.");
            }

        })
    }


    //============================================ Brand Tutor By Dilshad A. ==============================================

    // Brand Tutor Init

    $scope.addresourcebrandtutorinit = function () {

        $http.get('/Tutor/CoursesForBrandTutorResource').then(function (response) {
            $scope.CoursesForBrandTutor = response.data;

        })
    }

    // Upload Multiple files by tutors By Dilshad A. On 30 Sept 2020
    $scope.arrResourceFile = [];
    $scope.arrMultipleResourceFile = [];
    $scope.UploadResourseFilesByTutor = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrResourceFile.length; i++) {
                if ($scope.ResourceFileName == $scope.arrResourceFile[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrResourceFile.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrMultipleResourceFile.push(files);
        }
        else {
            alert('File is already added.')
        }
    };


    //============================================ Brand Tutor By Dilshad A. ==============================================

    // Upload Multiple files by tutors By Dilshad A. On 30 Sept 2020
    $scope.arrResourceFile = [];
    $scope.arrMultipleResourceFile = [];
    $scope.UploadResourseFilesByTutor = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrResourceFile.length; i++) {
                if ($scope.ResourceFileName == $scope.arrResourceFile[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrResourceFile.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrMultipleResourceFile.push(files);
        }
        else {
            alert('File is already added.')
        }
    };

    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteInternResourceArray = function (index) {
        var d = $scope.arrResourceFile[index];
        var d1 = $scope.arrMultipleResourceFile[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrResourceFile.splice(index, 1);
        $scope.arrMultipleResourceFile.splice(index, 1);
    }

    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByBrandTutor = function () {
        Upload.upload({
            url: "/Tutor/FileUploadByBrandTutor",
            data: {
                file: $scope.arrMultipleResourceFile,
                CourseId: $scope.CourseId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/Tutor/AddResourse";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };



    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteInternResourceArray = function (index) {
        var d = $scope.arrResourceFile[index];
        var d1 = $scope.arrMultipleResourceFile[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrResourceFile.splice(index, 1);
        $scope.arrMultipleResourceFile.splice(index, 1);
    }

    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByBrandTutor = function () {
        Upload.upload({
            url: "/Tutor/FileUploadByBrandTutor",
            data: {
                file: $scope.arrMultipleResourceFile,
                CourseId: $scope.CourseId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/Tutor/AddResourse";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };

    // ===============================================================================================================================

    // Change Students Profile Password

    $scope.ChangeStudentsProfilePassword = function () {

        if ($scope.NewPassword === $scope.ReTypePassword) {

            $http.post('/User/ChangeStudentsProfilePassword', {
                Email: $scope.Email, CurrentPassword: $scope.CurrentPassword,
                NewPassword: $scope.NewPassword, ReTypePassword: $scope.ReTypePassword
            }).then(function (response) {
                $scope.data = response.data;

                if (response.data == "currpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'Current Password is wrong',
                        icon: "warning",
                        dangerMode: "true"
                    })

                }
                else if (response.data == "passupdated") {

                    alert("Password Changed Successfully");
                    location.href = "/User/StudentLogin";
                }
                else if (response.data == "notregistered") {
                    swal({
                        title: 'Error!',
                        text: 'Intern not registered',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else if (response.data == "currnewpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'New Password and Retype Password must match',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else {
                    swal({
                        title: 'Error!',
                        text: 'Error while changing passsword Please try again',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
            })
        }
        else {
            swal({
                title: 'Error!',
                text: 'New Password and Retype Password must match',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    $scope.brandtutorChangePasswordInit = function () {

        $http.get('/User/GetBrandTutorEmail').then(function (response) {
            $scope.Email = response.data;
        })


    }

    //  ChangeStudentsProfilePassword

    // Change Brand Tutor Password

    $scope.ChangeBrandTutorPassword = function () {

        if ($scope.NewPassword === $scope.ReTypePassword) {

            $http.post('/User/ChangeBrandTutorPassword', {
                Email: $scope.Email, CurrentPassword: $scope.CurrentPassword,
                NewPassword: $scope.NewPassword, ReTypePassword: $scope.ReTypePassword
            }).then(function (response) {
                $scope.data = response.data;

                if (response.data == "currpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'Current Password is wrong',
                        icon: "warning",
                        dangerMode: "true"
                    })

                }
                else if (response.data == "passupdated") {

                    alert("Password Changed Successfully");
                    location.href = "/Tutor/BrandTutorLogin";
                }
                else if (response.data == "notregistered") {
                    swal({
                        title: 'Error!',
                        text: 'Intern not registered',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else if (response.data == "currnewpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'New Password and Retype Password must match',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else {
                    swal({
                        title: 'Error!',
                        text: 'Error while changing passsword Please try again',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
            })
        }
        else {
            swal({
                title: 'Error!',
                text: 'New Password and Retype Password must match',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    // Intern Tutor Change Password Init

    $scope.interntutorChangePasswordInit = function () {

        $http.get('/User/GetInternTutorEmail').then(function (response) {
            $scope.Email = response.data;

        })


    }



    // Intern Tutor Change Password

    $scope.ChangeInternTutorPassword = function () {
        if ($scope.NewPassword === $scope.ReTypePassword) {

            $http.post('/User/ChangeInternTutorPassword', {
                Email: $scope.Email, CurrentPassword: $scope.CurrentPassword,
                NewPassword: $scope.NewPassword, ReTypePassword: $scope.ReTypePassword
            }).then(function (response) {
                $scope.data = response.data;

                if (response.data == "currpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'Current Password is wrong',
                        icon: "warning",
                        dangerMode: "true"
                    })

                }
                else if (response.data == "passupdated") {

                    alert("Password Changed Successfully");
                    location.href = "/Tutor/InternTutorLogin";
                }
                else if (response.data == "notregistered") {
                    swal({
                        title: 'Error!',
                        text: 'Intern Tutor not registered',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else if (response.data == "currnewpasswrong") {
                    swal({
                        title: 'Error!',
                        text: 'New Password and Retype Password must match',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
                else {
                    swal({
                        title: 'Error!',
                        text: 'Error while changing passsword Please try again',
                        icon: "warning",
                        dangerMode: "true"
                    })
                }
            })
        }
        else {
            swal({
                title: 'Error!',
                text: 'New Password and Retype Password must match',
                icon: "warning",
                dangerMode: "true"
            })
        }
    }


    // Get Intern Tutor Id 

    $scope.GetInternTutorId = function () {

        $http.get('/User/GetInternTutorId').then(function (response) {
            $scope.Id = response.data;
            $scope.arrIdInternUserType = [];
            $scope.arrIdInternUserType.push({ Id: $scope.Id, UserType: "All-Intern-Tutor" });
            sessionStorage.setItem("InternIdUserNameForInternTut", JSON.stringify($scope.arrIdInternUserType));
        })


    }


    // Get Intern Communication By Id

    $scope.GetInternTutorCommunicationById = function (UserTye) {

        $scope.UsersType = "All-Intern-Tutor";

        $http.post('/User/GetAdminCommunicationForInternTutorById', { strUserType: $scope.UsersType }).then(function (response) {

            $scope.AdminCommtoInternTutorMsgByUserId = response.data;
            $scope.AdminToInternTutorCommunicationMsg = new NgTableParams({}, { dataset: $scope.AdminCommtoInternTutorMsgByUserId });
        });

    }
            
           
         

      



    //$scope.InterStucturePreview = function () {
    //    $("#interPreview").modal("show");
    //};

    // Course/Intern Content Preview


    $scope.CourseContentPreview = function () {
        var coursetype = $("#ddlcoursetype").val();
        if (coursetype === "Course") {
            $("#CourseContentPreview").modal("show");
        } else if (coursetype === "Intern") {
            $("#InternContentPreview").modal("show");
        }

    };
    // Brand Tutor Init

    $scope.BrandTutorCourse = function () {

        $http.get('/Tutor/BrandTutorCourse').then(function (response) {
            $scope.BrandTutor = response.data;
        })

    }


    //// Get Course List Details

    //$scope.getCoursesDetails = function (Id) {
    //    $scope.Id = Id;
    //    sessionStorage.CourseStructureId = JSON.stringify($scope.Id);
    //    location.href = "/User/CoursesListDetails";
    //}



    // Brand Tutor Course Detais

    //$scope.getBrandTutorsCourseDetails = function (Id) {

    //    sessionStorage.BrandStructureId = JSON.stringify(Id);
    //    location.href = "/Tutor/BrandTutorCourseList";

    //}


    $scope.getBrandTutorsCourseDetails = function (Id) {
        sessionStorage.BrandStructureId = JSON.stringify(Id);
        location.href = "/Tutor/BrandTutorCourseList";
    }


    $scope.GetBrandTutorCourseDetails = function () {
        if (sessionStorage.BrandStructureId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.BrandStructureId);
        }

        $http.get('/Tutor/GetBrandTutorCourseDetailsBasedOnId?Id=' + $scope.Id).then(function (response) {
            $scope.CourseBasedOnId = response.data;
            $scope.data = JSON.parse($scope.CourseBasedOnId);
            if ($scope.data["struc"] != '' && $scope.data["struc"] != undefined) {
                $scope.CourseName = $scope.data["struc"][0].CourseName;
                $scope.Fees = $scope.data["struc"][0].Fees;
                $scope.NetAmount = $scope.data["struc"][0].NetAmount;
                $scope.CategoryName = $scope.data["struc"][0].CategoryName;
                $scope.DurationName = $scope.data["struc"][0].DurationName;
                $scope.ValidFrom = $scope.data["struc"][0].ValidFrom;
                $scope.ValidTo = $scope.data["struc"][0].ValidTo;
                $scope.TutorId = $scope.data["struc"][0].TutorId;
                sessionStorage.TutorId = JSON.stringify($scope.TutorId);
            }


            // Course Content Details

            if ($scope.data["Heading"] != '' && $scope.data["Heading"] != undefined) {
                $scope.CourseContentHeading = $scope.data["Heading"][0].CourseContentHeading;
                $scope.SubHeading = $scope.data["Heading"][0].SubHeading;
                $scope.ShortDescription = $scope.data["Heading"][0].ShortDescription;
                $scope.DateCreated = $scope.data["Heading"][0].DateCreated;
            }

            // Storing description data in array

            $scope.descriptionarray = [];
            $scope.data7 = [];

            if ($scope.data["description"] != '' && $scope.data["description"] != undefined) {
                for (var i = 0; i < $scope.data["description"].length; i++) {
                    //  $scope.data7 = $scope.data["description"][i];
                    $scope.descriptionarray.push($scope.data["description"][i]);
                }

            }


            // Storing Prerequisities for course

            $scope.prerequisitearray = [];
            $scope.data8 = [];

            if ($scope.data["prerequisities"] != '' && $scope.data["prerequisities"] != undefined) {
                for (var i = 0; i < $scope.data["prerequisities"].length; i++) {
                    //  $scope.data8 = $scope.data["prerequisities"][i];
                    $scope.prerequisitearray.push($scope.data["prerequisities"][i]);
                }
            }


        })

    }


    $scope.BrandTutorCourseApplyPage = function () {
        window.location.href = "/User/StudentRegister";
    };


    $scope.StudentsOfBrandTutor = function () {

        $http.get('/Tutor/StudentsOfBrandTutor').then(function (response) {
            $scope.StudentsForBrandTutor = response.data;
            $scope.BrandTutorStudents = new NgTableParams({}, { dataset: $scope.StudentsForBrandTutor });
        })

    }


    // Bind Course For Brand Tutor

    $scope.CoursesForBrandTutor = function () {

        $http.get('/Tutor/CoursesForBrandTutor').then(function (response) {
            $scope.BrandTutorCourse = response.data;
        })
    }


    // Get Brand Tutor Course Students

    $scope.getBrandTutorCourseStudents = function () {

        $scope.CourseId = $scope.mdlCourseForStudent;

        $http.get('/Tutor/GetBrandTutorCourseStudents?Id=' + $scope.CourseId).then(function (response) {
            $scope.TutorCourseStudent = response.data;
            $scope.shListOfBrandStudent = true;
            $scope.BrandTutorStudentsList = new NgTableParams({}, { dataset: $scope.TutorCourseStudent });
        })
    }


    // Select Multiple Brand Student For COmmunication 

    $scope.arrBrandStudent = [];
    $scope.SelectMultipleBrandStudentForCommunication = function () {
        var data = $scope.TutorCourseStudent;
        $scope.arrBrandStudents = [];
        for (var i = 0; i < $scope.TutorCourseStudent.length; i++) {
            if ($scope.TutorCourseStudent[i].chkBrandStudent == true) {
                $scope.arrBrandStudents.push({ StudentId: data[i].StudentId, Name: data[i].Name, Email: data[i].Email, CourseId: data[i].CourseId });
            }
        }
    };


    // Add Communication For Brand Students 

    $scope.AddMessageForBrandStudentsCommunicatioin = function () {


        if ($scope.chkAllBrandTutorStudents === undefined || $scope.chkAllBrandTutorStudents === "") {
            $scope.chkAllBrandTutorStudents = false;
        }

        $http.post("/Tutor/AddMessageForBrandStudentsCommunicatioin",
            {

                CourseId: $scope.mdlCourseForStudent, BrandTutorCourseId: $scope.CourseId, IsAllBrandStudent: $scope.chkAllBrandTutorStudents, lstBrandStudent: $scope.arrBrandStudents,
                StudentMsg: $scope.mdlMsgToStudents, StudentMsgToAll: $scope.mdlMsgAll
                //StudentCategoryId:$scope.
            }).then(function (resAddComm) {
                if (resAddComm.data > 0) {
                    alert("Message successfully sent.");
                    window.location.href = "/Tutor/BrandTutorCommToStudents";

                }
                else {
                    console.log(resAddComm.data);
                    alert("Failed. Something went wrong.");
                }
            });
    }


    $scope.GetCommunicationData = function () {

        $http.get('/Tutor/GetCommunicationData').then(function (response) {
            $scope.BrandTutorData = response.data;
            $scope.TutorsCommunicationData = new NgTableParams({}, { dataset: $scope.BrandTutorData });

        })
    }


    // View Resource Details(Uploaded by Brand Tutor) By Dilshad A. on 06 Oct 2020 
    $scope.ViewResourceDetails = function () {
        $http.get("/Tutor/ViewResourceDetails").then(function (res) {
            $scope.ResourceTutor = res.data;
            $scope.ResourceTutorTable = new NgTableParams({}, { dataset: $scope.ResourceTutor });
        });
    }


    // Student to brand tutor communication

    $scope.BrandStudentCommunicationToTutor = function () {

        $scope.hideBrandStudentCommunication = true;
        $scope.brandtutorcomm = false;

        $http.get('/User/BrandStudentCommunicationToTutor').then(function (response) {
            $scope.StudentsBrandTutorData = response.data;
        })
    }


    //$http.get('/User/BrandStudentCommunicationToTutor').then(function (response) {
    //    $scope.StudentsBrandTutorData = response.data;
    //})

    // Brand Student Communication To Brand Tutor

    $scope.AddStudentCommunicationToBrandTutor = function () {

        $scope.StudentCommToTutor = {};

        $scope.StudentCommToTutor.Id = $scope.BrandTutor;

        $scope.StudentCommToTutor.Message = $scope.mdlMsgOnlyTutor;

        // $scope.BrandTutor;
        // $scope.mdlMsgOnlyTutor;

        $http.post('/User/AddStudentCommunicationToBrandTutor', { objbrandstudenttotutor: $scope.StudentCommToTutor }).then(function (response) {

            $scope.CommunicationMessageForTutorData = response.data;

            if (response.data > 0) {
                alert("Message successfully sent.");
                window.location.href = "/User/BrandStudentCommToTutors";
            }
            else {
                console.log(response.data);
                alert("Failed. Something went wrong.");
            }

        })


        //     = $scope.BrandTutor;
        //     = $scope.mdlMsgOnlyTutor;

    }

    $scope.BrandStudentMessagesToTutors = function () {

        $http.get('/User/BrandStudentMessagesToTutors').then(function (response) {

            $scope.BrandStudentToTutorsData = response.data;
            $scope.BrandStudentMsgToTutoursData = new NgTableParams({}, { dataset: $scope.BrandStudentToTutorsData });

        })

    }


    // Get Student Message Of Brand Tutor

    $scope.GetStudentMessageOfBrandTutor = function () {

        $http.post('/Tutor/GetStudentMessageOfBrandTutor', { userType: "All-Brand-Tutor" }).then(function (response) {
            $scope.StudentMessageOfBrandTutor = response.data;

            $scope.StudentMessageToBrandTutorByUserId = response.data.data;
            $scope.BrandStudentMessageToTutours = new NgTableParams({}, { dataset: $scope.StudentMessageToBrandTutorByUserId });
            // $scope.StudentMessageToBrandTutorForAll = response.data.msgAll;

        })
    }


    $scope.TutorCommToStudentInit = function () {

        $scope.hideCourseStudent = true;
        $scope.hideBrandStudent = true;
        $scope.hideUnregisteredStudent = true;

        $http.get('/User/CourseStudents').then(function (response) {
            $scope.CourseStudents = response.data;
            if (response.data > 0) {
                $scope.hideCourseStudent = false;
            }
            $scope.CoursetudentsList = new NgTableParams({}, { dataset: $scope.CourseStudents });
        })

        $http.get('/User/UnregisteredStudentsInCourse').then(function (response) {
            $scope.StudentsWithoutCourse = response.data;
            if (response.data > 0) {
                $scope.hideUnregisteredStudent = false;
            }
            $scope.StudentsListUnregisteredInCourse = new NgTableParams({}, { dataset: $scope.StudentsWithoutCourse });

        })


        $http.get('/User/BrandStudents').then(function (response) {
            $scope.BrandStudentsData = response.data;
            if (response.data > 0) {
                $scope.hideBrandStudent = false;
            }
            $scope.BrandStudentsList = new NgTableParams({}, { dataset: $scope.BrandStudentsData });


        })

    }

    // Get Student Type

    $scope.getStudentType = function () {

        if ($scope.StudentType == "Student") {
            $scope.hideCourseStudent = true;
            $scope.hideBrandStudent = true;
            $scope.hideUnregisteredStudent = false;
        }

        else if ($scope.StudentType == "Course Student") {
            $scope.hideUnregisteredStudent = true;
            $scope.hideCourseStudent = false;
            $scope.hideBrandStudent = true;
        }

        else if ($scope.StudentType == "Brand Student") {
            $scope.hideUnregisteredStudent = true;
            $scope.hideBrandStudent = false;
            $scope.hideCourseStudent = true;
        }

        else if ($scope.StudentType == "") {
            $scope.hideUnregisteredStudent = true;
            $scope.hideBrandStudent = true;
            $scope.hideCourseStudent = true;
        }


    }




    //$scope.arrBrandStudent = [];
    $scope.SelectMultipleBrandStudentForTutorStudentCommunication = function () {
        var data = $scope.BrandStudentsData;
        $scope.arrBrandStudentsArray = [];
        for (var i = 0; i < $scope.BrandStudentsData.length; i++) {
            if ($scope.BrandStudentsData[i].chkBrandStudents == true) {
                $scope.arrBrandStudentsArray.push({
                    StudentId: data[i].StudentId, Name: data[i].Name, Email: data[i].Email, TutorId: data[i].TutorId, StudentCategoryId: $scope.StudentCategoryId,
                    CourseStructureId: data[i].CourseStructureId
                });
            }
        }
    };



    //CourseId: 2067
    //Email: "atulsha628@gmail.com"
    //Name: "Abhay Sir"
    //StudentId: 10019
    // StudentCategoryId

    $scope.SelectMultipleCourseStudentForTutorStudentCommunication = function () {
        var data = $scope.CourseStudents;
        $scope.arrCourseArrayForTutorStudent = [];
        for (var i = 0; i < $scope.CourseStudents.length; i++) {
            if ($scope.CourseStudents[i].chkCourseStudents == true) {
                $scope.arrCourseArrayForTutorStudent.push({
                    StudentId: data[i].StudentId, Name: data[i].Name, Email: data[i].Email, CourseId: data[i].CourseId, StudentCategoryId: $scope.StudentCategoryId,

                });
            }
        }
    };



    // Tutor To Student Add Communication

    $scope.TutorsToStudentAddCommunicatioin = function () {

        $scope.TutorStudentCommunication = {};

        $scope.TutorStudentCommunication.IsAllStudent = $scope.chkAllStudents;
        $scope.TutorStudentCommunication.CategoryType = $scope.StudentType;
        $scope.TutorStudentCommunication.Message = $scope.mdlMsgToStudents;

        if ($scope.chkAllStudents == "" || $scope.chkAllStudents == undefined) {
            $scope.TutorStudentCommunication.IsAllStudent = false;
        }
        else {
            $scope.TutorStudentCommunication.IsAllStudent = $scope.chkAllStudents;
        }


        $http.post('/User/TutorsToStudentAddCommunicatioin', {
            objTutorComm: $scope.TutorStudentCommunication, arrTutorBrandStudents: $scope.arrBrandStudentsArray, arrTutorCourseStudents: $scope.arrCourseArrayForTutorStudent
        }).then(function (response) {
            $scope.TutorsToStudentCommunication = response.data;

            if (response.data > 0) {
                alert("Message successfully sent.");
                window.location.href = "/User/ViewTutorCommToStudent";

            }
            else {
                console.log(resAddComm.data);
                alert("Failed. Something went wrong.");
            }
        })

    }


    // BrandStudentMessagesToTutors

    // Tutor To Student Outgoing message
    $scope.TutorToStudentMessage = function () {

        $http.get('/User/TutorToStudentMessage').then(function (response) {

            //$scope.BrandStudentToTutorsData = response.data;
            //$scope.BrandStudentMsgToTutoursData = new NgTableParams({}, { dataset: $scope.BrandStudentToTutorsData });

            $scope.TutorToStudentsCommunicationData = response.data;
            $scope.TutorToStudentsMsgData = new NgTableParams({}, { dataset: $scope.TutorToStudentsCommunicationData });

        })

    }



    //$scope.InternListTutorinit = function () {

    //    $http.get('/Admin/InterListTutorsData').then(function (response) {
    //        $scope.InternTutorList = response.data;
    //    })

    //}


    $scope.CourseListTutorinit = function () {

        $http.get('/Admin/CourseListTutorsData').then(function (response) {
            $scope.CourseTutorList = response.data;
        })

    }

    //$scope.getInternTutorDetails = function (Id) {

    //    $scope.Id = Id;
    //    sessionStorage.InternshipsId = JSON.stringify($scope.Id);
    //    location.href = "/Internship/AddTutorResourcesForInternship";
    //}

    $scope.getCourseTutorDetails = function (Id) {

        $scope.Id = Id;
        sessionStorage.CoursesId = JSON.stringify($scope.Id);
        location.href = "/Admin/AddTutorResourcesForCourse";
    }



    $scope.InternTutorArray = [];
    $scope.getCourseTutorDetailsOnId = function () {
        if (sessionStorage.CoursesId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.CoursesId);
        }

        $http.get(`/Admin/GetTutorResourcesForCourse?CourseId=${$scope.Id}`).then(function (response) {
            $scope.data = JSON.parse(response.data);

            if ($scope.data["intern"] != '' && $scope.data["intern"] != undefined) {
                $scope.CourseName = $scope.data["intern"][0].CourseName;
            }

            $scope.CourseArray = [];

            if ($scope.data["intrtutor"] != '' && $scope.data["intrtutor"] != undefined) {

                for (var i = 0; i < $scope.data["intrtutor"].length; i++) {
                    $scope.CourseArray.push($scope.data["intrtutor"][i]);
                }
            }

            $scope.multiplefilesarray = [];
            if ($scope.data["resource"] != '' && $scope.data["resource"] != undefined) {
                for (var i = 0; i < $scope.data["resource"].length; i++) {
                    $scope.multiplefilesarray.push({ ResourceName: $scope.data["resource"][i].ResourceName, ResourceId: $scope.data["resource"][i].ResourceId, CourseId: $scope.data["resource"][i].CourseId });
                }
            }



        });


        $http.get("/Admin/GetTutorsData").then(function (response) {
            $scope.TutorsData = response.data;
        })
    }


    $scope.CourseArray = [];
    $scope.AddMultipleTutorForCourse = function () {
        $scope.CourseArray.push({ CourseId: $scope.Id, CourseName: $scope.CourseName, TutorId: $scope.mdlTutor.TutorId, TutorName: $scope.mdlTutor.TutorName });
    }


    $scope.deleteCourseTutorArray = [];

    $scope.DeleteCourseTutorArray = function (index) {

        var data2 = $scope.CourseArray[index];
        $scope.deleteCourseTutorArray.push({ CourseId: data2.CourseId, TutorId: data2.TutorId, CourseTutorId: data2.CourseTutorId });
        $scope.CourseArray.splice(index, 1);
    }


    $scope.deleteCourseResourceArray = [];
    $scope.DeleteCourseResourceArray = function (index) {

        var data2 = $scope.multiplefilesarray[index];
        $scope.deleteCourseResourceArray.push({ CourseId: data2.CourseId, CourseName: data2.CourseName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.multiplefilesarray.splice(index, 1);
    }


    // Add Multiple Tutor Resources For Internship

    $scope.AddTutorResourceForCourse = function () {

        $http.post('/Internship/AddTutorForCourse', { CourseArray: $scope.CourseArray }).then(function (response) {
            $scope.data = response.data;
        })
    }


    // Add Multiple Tutor Resources For Internship

    $scope.AddTutorResourceForCourse = function () {
        Upload.upload({
            url: "/Admin/AddTutorForCourse",
            method: "POST",
            data: {
                CourseArray: $scope.CourseArray,
                Filearray: $scope.multiplefilesarray,
                CourseId: $scope.Id,
                deletecoursetutor: $scope.deleteCourseTutorArray,
                deleteCourseResourceArray: $scope.deleteCourseResourceArray
            }
        }).then(function (response) {
            $scope.data = response.data;

            if (response.data == "Done") {
                alert("Data Inserted Successfully");
                location.href = "/Admin/CourseListTutor";
            }
        })
    }



    $scope.UploadTheFiles = function (files) {

        $scope.SelectedFilesCourse = files;

        for (var i = 0; i < $scope.SelectedFilesCourse.length; i++) {
            if ($scope.SelectedFilesCourse[i].type == "application/x-msdownload" || $scope.SelectedFilesCourse[i].type == "video/mp4") {
                alert("Can't upload video and exe");
            }
            else {
                //  $scope.Filearray.push($scope.SelectedFiles[i].name);            
                $scope.multiplefilesarray.push({ Files: $scope.SelectedFilesCourse, ResourceName: $scope.SelectedFilesCourse[i].name, CourseId: $scope.Id });
                Upload.upload({
                    url: "/Admin/Upload",
                    data: {
                        files: $scope.SelectedFilesCourse
                    }
                }).then(function (response) {
                    $scope.data = response.data;

                });
            }
        }
    }




    // Add Student Feedback Details by Dilshad A. on 19 Oct 2020

    $scope.AddStudentFeedbackDetails = function () {
        $scope.clsStdFeed = {};
        $scope.clsStdFeed.Subject = $scope.mdlStudentFeedSub;
        $scope.clsStdFeed.Message = $scope.mdlStduentFeedMsg;
        $http.post("/User/AddStudentFeedbackDetails", { objFeedback: $scope.clsStdFeed }).then(function (res) {
            $scope.StuFeedMsg = res.data;
            if (res.data > 0) {
                alert("Feedback successfully added.");
                locatiion.href = "/User/AddStudentFeedback";
            }
            else {
                alert("Failed. Something went wrong!!!");
            }
        });
    };



    // View Feedback Details For Admin By Dilshad A. on 27 Oct 2020

    $scope.ViewFeedbackDetails = function () {
        $http.get("/Admin/ViewFeedbackDetails").then(function (response) {
            $scope.Feedback = response.data;
            $scope.FeedbackTable = new NgTableParams({}, { dataset: $scope.Feedback });
        });
    };


    // Course Student to Tutor

    $scope.CourseStudentToTutorMsg = function () {

        $http.get('/User/CourseTutors').then(function (response) {
            $scope.CourseTutorsData = response.data;
        })
    }


    // Intern Tutor Init Get Intern Student details

    $scope.GetInternStudents = function () {
        $http.get('/InternTutor/GetInternStudents').then(function (response) {
            $scope.InternStudentsData = response.data;
            $scope.InternStudentsTable = new NgTableParams({}, { dataset: $scope.InternStudentsData });

        })
    }


    $scope.InternTutorInit = function () {

        $http.get('/InternTutor/Internship').then(function (response) {
            $scope.InternshipsData = response.data;
        })
    }


    // Get Brand Tutor Course Students

    //$scope.getBrandTutorCourseStudents = function () {

    //    $scope.CourseId = $scope.mdlCourseForStudent;

    //    $http.get('/Tutor/GetBrandTutorCourseStudents?Id=' + $scope.CourseId).then(function (response) {
    //        $scope.TutorCourseStudent = response.data;
    //        $scope.shListOfBrandStudent = true;
    //        $scope.BrandTutorStudentsList = new NgTableParams({}, { dataset: $scope.TutorCourseStudent });
    //    })
    //}

    // Get Students for internship for intern student

    $scope.getInternTutorStudents = function () {

        $scope.InternId = $scope.mdlInternForStudent;

        $http.get('/InternTutor/GetInternTutorStudents?Id=' + $scope.InternId).then(function (response) {
            $scope.InternStudentsForInternship = response.data;
            $scope.shListOfInternStudent = true;
            $scope.InternTutorStudentsList = new NgTableParams({}, { dataset: $scope.InternStudentsForInternship });
        })
    }


    // Select Multiple Intern Student For Communication 

    $scope.arrInternStudentForInternship = [];
    $scope.SelectMultipleInternStudentsForCommunication = function () {
        var data = $scope.InternStudentsForInternship;
        $scope.arrInternStudentForInternships = [];
        for (var i = 0; i < $scope.InternStudentsForInternship.length; i++) {
            if ($scope.InternStudentsForInternship[i].chkInternStudents == true) {
                $scope.arrInternStudentForInternships.push({ StudentId: data[i].InterApllyId, Name: data[i].Name, Email: data[i].Email, InternshipId: data[i].InternshipId });
            }
        }
    };


    // Add Communication For Intern Students 

    $scope.AddMessageForInternStudentCommunication = function () {


        if ($scope.chkAllInternStudents === undefined || $scope.chkAllInternStudents === "") {
            $scope.chkAllInternStudents = false;
        }

        $http.post("/InternTutor/AddMessageForInternStudentCommunication",
            {

                InternshipId: $scope.InternId, IsAllInternStudent: $scope.chkAllInternStudents, lstInternStudent: $scope.arrInternStudentForInternships,
                StudentMsg: $scope.mdlMsgToInternStudents
                //StudentCategoryId:$scope.
            }).then(function (resAddComm) {
                if (resAddComm.data > 0) {
                    alert("Message successfully sent.");
                    window.location.href = "/InternTutor/ViewCommunication";

                }
                else {
                    console.log(resAddComm.data);
                    alert("Failed. Something went wrong.");
                }
            });
    }


    // View Messages Sent To Intern Student by Intern Tutor Init

    $scope.GetMessagesToInternStudent = function () {
        $http.get('/InternTutor/GetMessagesToInternStudent').then(function (response) {
            $scope.InternStudentsDetail = response.data;
            $scope.InternStudentDataTable = new NgTableParams({}, { dataset: $scope.InternStudentsDetail });
        })
    }


    $scope.GetInternStudentCommByTutor = function (UserTye) {

        if (UserTye === 'All-Intern-Students') {
            $scope.Usertype = "All-Intern-Students";
            $http.post("/User/GetInternStudentCommByTutor", { strUserType: $scope.Usertype }).then(function (res) {
                $scope.InternStudentMsgByInternTutor = res.data.data;
                $scope.InternTutorToInternStudentMessages = new NgTableParams({}, { dataset: $scope.InternStudentMsgByInternTutor });
                //  $scope.CommForAll = res.data.msgAll;
            });
        }

    }


    // Intern Student to Intern Tutor 

    $scope.InternStudentToInternTutorInit = function () {

        $http.get('/User/InternTutorsForInternStudent').then(function (response) {
            $scope.InternTutorsData = response.data;
        })


    }

    // Intern Student To Intern Tutor Add Communication

    $scope.AddMessageForStudentToInternTutorCommunication = function () {

        //    $scope.StudentToInternTutor = {};
        //    $scope.StudentToInternTutor.TutorId = parseInt($scope.mdlInternshipTutor);
        //    $scope.StudentToInternTutor.Message = $scope.mdlMsgToInternStudents;

        $http.post('/User/AddMessageForStudentToInternTutorCommunication', {
            TutorId: $scope.mdlInternshipTutor,
            Message: $scope.mdlMsgToInternStudents
        }).then(function (response) {

            if (response.data > 0) {
                alert("Message successfully sent.");
                window.location.href = "/User/ViewStudentToInternTutorCommunication";

            }
            else {
                console.log(response.data);
                alert("Failed. Something went wrong.");
            }

        })

    }


    // Init for Intern Student Outgoing message

    $scope.GetInternStudentToInternTutorComm = function () {

        $http.get('/User/GetInternStudentToInternTutorComm').then(function (response) {
            $scope.InternStudentsOutgoingComm = response.data;
            $scope.InternStudentsOutgoingToInternTutor = new NgTableParams({}, { dataset: $scope.InternStudentsOutgoingComm });
        })
    }

    // Get Intern Student Message To Intern Tutor

    $scope.GetInternStudentMessage = function () {

        $scope.UserType = "All-Intern-Tutor";
        $http.get('/InternTutor/GetInternStudentMessage?strUserType= ' + $scope.UserType).then(function (response) {
            $scope.InternStudentMessages = response.data;
            $scope.InternStudentMsgIncomingForInternTutor = new NgTableParams({}, { dataset: $scope.InternStudentMessages });
        })

    }



    // Course Students Init

    $scope.CourseStudentsInit = function () {

        $scope.hidestrCourse = true;

        $http.get('/User/CourseTutorss').then(function (response) {
            $scope.CourseStudentTutorsData = response.data;
        })

    }





    // Get Courses Of Course Tutor

    $scope.getCoursesOfCourseTutor = function () {

        $scope.TutorId = $scope.CourseTutor;

        $scope.hidestrCourse = false;

        $http.get('/User/GetCourseOfCourseTutor?Id=' + $scope.TutorId).then(function (response) {
            $scope.GetCourseTutorsForCourseStudent = response.data;
            //    $scope.shListOfInternStudent = true;
            //    $scope.InternTutorStudentsList = new NgTableParams({}, { dataset: $scope.InternStudentsForInternship });
        })

    }


    // Course Student to Tutor

    $scope.CourseStudentToTutorAddCommunication = function () {

        $http.post('/User/CourseStudentToTutorAddCommunication', {
            TutorId: $scope.TutorId,
            CourseId: $scope.strCourse,
            Message: $scope.mdlMessageToCourseTutor,

        }).then(function (response) {

            if (response.data > 0) {
                alert("Message successfully sent.");
                window.location.href = "/User/CourseStudentToTutorOutgoing";

            }
            else {
                console.log(response.data);
                alert("Failed. Something went wrong.");
            }

        })

    }




    // Get Course Student Message To Course Tutor

    $scope.GetCourseStudentMessage = function () {

        $scope.UserType = "All-Students-Course";
        $http.get('/User/GetCourseTutorIncoming?strUserType= ' + $scope.UserType).then(function (response) {
            $scope.CourseStudentMessages = response.data;
            $scope.CourseStudentMsgIncomingForCourseTutor = new NgTableParams({}, { dataset: $scope.CourseStudentMessages });
        })

    }


    // Get Course Tutor to Course Student Message ( Tutor Outgoing Message To Course Student )

    $scope.GetCourseTutorToCourseStudentMessage = function () {

        $http.get('/User/GetTutorCommunicationToStudent').then(function (response) {
            $scope.CourseTutorToCourseStudentMessages = response.data;
            $scope.CourseTutorOutgoingMessageForStudent = new NgTableParams({}, { dataset: $scope.CourseTutorToCourseStudentMessages });
        })

    }


    // Get Tutor To Course Student Incoming Message

    $scope.GetTutorToCourseStudentIncomingComm = function () {

        $scope.usertype = "All-Students-Course";
        $http.get('/User/GetTutorToCourseStudentIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.TutorToCourseStudentIncomingComm = response.data;
            $scope.TutorToCourseStudentIncomingMessages = new NgTableParams({}, { dataset: $scope.TutorToCourseStudentIncomingComm });
        })
    }

    // Get Course Student To Course Tutor Outgoing Message ( Course Student Outgoing Message To Course Tutor )
    $scope.GetStudentCommunicationToCourseTutor = function () {

        $http.get('/User/GetStudentCommunicationToCourseTutor').then(function (response) {
            $scope.CourseStudentToCourseTutorOutgoingMsg = response.data;
            $scope.CourseStudentOutgoingMessageForTutors = new NgTableParams({}, { dataset: $scope.CourseStudentToCourseTutorOutgoingMsg });
        });
    };

    // Get Course Duration by Dilshad A. on 02 Nov 2020
    $scope.GetDuration = function () {
        $http.get('/Course/GetDuration').then(function (response) {
            $scope.Durations = response.data;
        });
    };

    // Get Coursed by Student Category Id and Student Id By Dilshad A. on 02 Nov 2020
    $scope.GetCoursesByStudentCategoryId = function () {        
        $scope.hideSavebtn = true;
        $scope.hidecourseValidation = true;

        $http.get("/User/GetCoursesByStudentCategoryId").then(function (res) {
            $scope.CourseDataDeatils = res.data;
        });

        $http.get('/Course/GetDuration').then(function (response) {
            $scope.DurationData = response.data;

        })
    }


    // Get Brand Tutor To Brand Student Incoming Message

    $scope.GetTutorToCourseStudentIncomingComm = function () {

        $scope.usertype = "All-Brand-Students";
        $http.get('/User/GetBrandTutorToBrandStudentIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.BrandTutorToBrandStudentIncomingComm = response.data;
            $scope.BrandTutorToBrandStudentIncomingMessages = new NgTableParams({}, { dataset: $scope.BrandTutorToBrandStudentIncomingComm });
        })
    }


    // Student adding course after login (multiple courses allowed to take 5/11/2020 Atul Sh.)

    $scope.AddStudentCourses = function () {

        $http.post('/User/AddStudentCourses', { multicourse: $scope.arrCourse }).then(function (response) {
            $scope.MultipleCourseForStudent = response.data;

            if (response.data == true) {
                alert("Course taken successfully");
                location.href = "/User/CourseList";
            }
            else if (response.data == false) {
                alert("Error while adding course");
            }

        });
    }


    // Get Brand Tutor To Brand Student Incoming Message

    $scope.AdminToCourseStudentIncomingComm = function () {

        $scope.usertype = "All-Students-Course";
        $http.get('/User/AdminToCourseStudentIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.AdminToCourseStudentIncomingComm = response.data;
            $scope.AdminToCourseStudentIncomingMessages = new NgTableParams({}, { dataset: $scope.AdminToCourseStudentIncomingComm });
        })
    }


    $scope.AdminToInternStudentIncomingComm = function () {

        $scope.usertype = "All-Intern-Students";
        $http.get('/User/AdminToInternStudentIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.AdminToInternStudentIncomingComm = response.data;
            $scope.AdminToInternStudentIncomingMessages = new NgTableParams({}, { dataset: $scope.AdminToInternStudentIncomingComm });
        })
    }

    $scope.AdminToCourseTutorIncomingComm = function () {

        $scope.usertype = "All-Course-Tutor";
        $http.get('/User/AdminToCourseTutorIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.AdminToCourseTutorIncomingComm = response.data;
            $scope.AdminToCourseTutorIncomingMessages = new NgTableParams({}, { dataset: $scope.AdminToCourseTutorIncomingComm });
        })
    }

    // Update Intern Profile Qualification 

    $scope.UpdateInternProfileQualification = function () {

        $scope.UpdateIntenrProfileCommunication = {};


        $scope.UpdateIntenrProfileCommunication.HowDoYouKnow = $scope.HowDoYouKnow;
        $scope.UpdateIntenrProfileCommunication.YearOfPassing = $scope.YearOfPassing;
        $scope.UpdateIntenrProfileCommunication.Qualification = $scope.Qualification;
        $scope.UpdateIntenrProfileCommunication.CollegeUniv = $scope.CollegeUniv;


        // $scope.UpdateIntenrProfileCommunication.Comments = $scope.Comments;

        $http.post('/User/UpdateInternProfileQualification', { updateinternqualification: $scope.UpdateIntenrProfileCommunication}).then(function (response) {
            $scope.InternsProfileQualification = response.data; 
            location.href = "/User/InternProfile";
            if (response.data == true) {
                alert("Intern Qualification updated successfully");
            }
            else if (response.data == false) {
                alert("Error while updating Intern Qualification");
            }

            else if (response.data == "problem") {
                alert("Problem while updating Intern Qualification");
            }

        })
    }


    // Update Intern Profile Document

    $scope.UpdateInternProfileDocument = function () {

        $scope.arrInternApplyDoc = [];

        if ($scope.resumeFileName !== undefined && $scope.resumeFileName != "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.PdfDocumentId, UserId: $scope.UserIdPdf, DocumentName: $scope.resumeFileName, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
        }
        else if ($scope.fileResume !== undefined && $scope.fileResume != "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.PdfDocumentId, UserId: $scope.UserIdPdf, DocumentName: $scope.fileResume, DocumentType: "Resume", DoucmentNo: "No-Doc-Number-Resume" });
        }

        if ($scope.mdlInternApplyAadhar != undefined && $scope.mdlInternApplyAadhar != "" && $scope.fileAadhar.$ngfName === undefined || $scope.fileAadhar.$ngfName === "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.AadharDocumentId, UserId: $scope.UserIdAadhar, DocumentName: $scope.fileAadhar, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
        }

        else if ($scope.mdlInternApplyAadhar != undefined && $scope.mdlInternApplyAadhar != "" && $scope.aadhaarFileName !== undefined || $scope.aadhaarFileName !== "") {
            $scope.arrInternApplyDoc.push({ DocumentId: $scope.AadharDocumentId, UserId: $scope.UserIdAadhar, DocumentName: $scope.aadhaarFileName, DocumentType: "AAdhaar", DoucmentNo: $scope.mdlInternApplyAadhar });
        }


        Upload.upload({
            url: "/User/UpdateInternProfileDocument",
            data: {
                
                lstDocuments: $scope.arrInternApplyDoc,
                resumePostedfile: $scope.resumeFullfile,
                aadhaarPostedfile: $scope.aadhaarFullfile
            }
        }).then(function (response) {
            $scope.data = response.data;
            if (response.data == "Updated") {
                alert("Documents Updated Successfully");
                location.href = "/User/InternshipDocument";
            }
            else if (response.data == "Error") {
                alert("Error while updating Documents");
            }
            
        })
    }


    // Upload Multiple files by tutors By Dilshad A. On 30 Sept 2020
    $scope.arrResourceFiles = [];
    $scope.arrMultipleResourceFiles = [];
    $scope.UploadResourseFilesByTutors = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrResourceFiles.length; i++) {
                if ($scope.ResourceFileName == $scope.arrResourceFiles[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrResourceFiles.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrMultipleResourceFiles.push(files);
        }
        else {
            alert('File is already added.')
        }
    };


    //============================================ Brand Tutor By Dilshad A. ==============================================

    // Upload Multiple files by tutors By Dilshad A. On 30 Sept 2020
    $scope.arrResourceFiles = [];
    $scope.arrMultipleResourceFiles = [];
    $scope.UploadResourseFilesByTutors = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrResourceFiles.length; i++) {
                if ($scope.ResourceFileName == $scope.arrResourceFiles[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrResourceFiles.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrMultipleResourceFiles.push(files);
        }
        else {
            alert('File is already added.')
        }
    };


    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByTutor = function () {
        Upload.upload({
            url: "/Tutor/FileUploadByTutor",
            data: {
                file: $scope.arrMultipleResourceFiles,
                CourseId: $scope.CourseId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/Tutor/AddTutorResource";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };


    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByTutor = function () {
        Upload.upload({
            url: "/Tutor/FileUploadByTutor",
            
            data: {
                file: $scope.arrMultipleResourceFiles,
                CourseId: $scope.CourseId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/Tutor/AddTutorResource";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };


    // View Tutor Resource Details

    $scope.ViewTutorResourceDetails = function () {

        $http.get('/Tutor/ViewTutorResourceDetails').then(function (response) {
            $scope.ViewTutorResources = response.data;
            $scope.ViewTutorResourceDetails = new NgTableParams({}, { dataset: $scope.ViewTutorResources });
        });

    }


    // Download Tutor Resources For Course Student

    $scope.DownloadStudentResourcesForCourseStudent = function () {

        $http.get('/Tutor/DownloadStudentResourcesForCourseStudent').then(function (response) {
            $scope.DownloadTutorResourcesForCourseStudent = response.data;
            $scope.DownloadTutorResourcesForCourseStudentTable = new NgTableParams({}, { dataset: $scope.DownloadTutorResourcesForCourseStudent });
        })
    }


    // Download Tutor Resources For Brand Student

    $scope.DownloadResourcesForBrandStudent = function () {

        $http.get('/Tutor/DownloadResourcesForBrandStudent').then(function (response) {
            $scope.DownloadTutorResourcesForBrandStudent = response.data;
            $scope.DownloadTutorResourcesForBrandStudentTable = new NgTableParams({}, { dataset: $scope.DownloadTutorResourcesForBrandStudent });
        })
    }


    // Download Tutor Resources For Intern Student

    $scope.DownloadResourcesForInternStudent = function () {

        $http.get('/InternTutor/DownloadResourcesForInternStudent').then(function (response) {
            $scope.DownloadTutorResourcesForInternStudent = response.data;
            $scope.DownloadTutorResourcesForInternStudentTable = new NgTableParams({}, { dataset: $scope.DownloadTutorResourcesForInternStudent });
        })
    }
    


    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteTutorsResourceArray = function (index) {
        var d = $scope.arrResourceFiles[index];
        var d1 = $scope.arrMultipleResourceFiles[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrResourceFiles.splice(index, 1);
        $scope.arrMultipleResourceFiles.splice(index, 1);
    }

    
    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteTutorsResourceArray = function (index) {
        var d = $scope.arrResourceFiles[index];
        var d1 = $scope.arrMultipleResourceFiles[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrResourceFiles.splice(index, 1);
        $scope.arrMultipleResourceFiles.splice(index, 1);
    }

    $scope.addtutorresourceinit = function () {

        $http.get('/Tutor/CourseForTutor').then(function (response) {
            $scope.CourseForTutor   = response.data;
        })
    }



    // ===================== Add Resource For Intern Tutor by Atul Sh. 9 November====================================


    $scope.arrInternTutorResourceFiles = [];
    $scope.arrInternTutorMultipleResourceFiles = [];
    $scope.UploadInternTutorResourceFiles = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrInternTutorResourceFiles.length; i++) {
                if ($scope.ResourceFileName == $scope.arrInternTutorResourceFiles[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrInternTutorResourceFiles.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrInternTutorMultipleResourceFiles.push(files);
        }
        else {
            alert('File is already added.')
        }
    };


    //============================================ Intern Tutor By Atul Sharma ==============================================

    // Upload Multiple files by tutors By Dilshad A. On 30 Sept 2020

    $scope.arrInternTutorResourceFiles = [];
    $scope.arrInternTutorMultipleResourceFiles = [];
    $scope.UploadInternTutorResourceFiles = function (files) {
        $scope.ResourceFileName = files.name;
        //$scope.SelectedFiles = files;

        var count = 0;
        if ($scope.ResourceFileName != undefined) {
            for (var i = 0; i < $scope.arrInternTutorResourceFiles.length; i++) {
                if ($scope.ResourceFileName == $scope.arrInternTutorResourceFiles[i].ResourceName) {
                    count++;
                }
            }
        }
        else {
            alert('Please add file.')
        }

        if (count == 0) {
            $scope.arrInternTutorResourceFiles.push({ ResourceName: $scope.ResourceFileName });
            $scope.arrInternTutorMultipleResourceFiles.push(files);
        }
        else {
            alert('File is already added.')
        }
    };


    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByInternTutor = function () {
        Upload.upload({
            url: "/InternTutor/FileUploadByInternTutor",
            data: {
                file: $scope.arrInternTutorMultipleResourceFiles,
                InternshipId: $scope.InternshipId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/InternTutor/AddInternTutorResource";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };


    // File Upload By Brand Tutor By Dilshad A. on 03 Oct 2020
    $scope.FileUploadByInternTutor = function () {
        Upload.upload({
            url: "/InternTutor/FileUploadByInternTutor",

            data: {
                file: $scope.arrInternTutorMultipleResourceFiles,
                InternshipId: $scope.InternshipId
            }
        }).then(function (res) {
            if (res.data == "Successfully") {
                alert("File(s) successfully uploaded.");
                location.href = "/InternTutor/AddInternTutorResource";
            }
            else {
                alert("Something went wrong. Please try again!!!");
            }
        });
    };


    // View Tutor Resource Details

    $scope.ViewTutorResourceDetails = function () {

        $http.get('/Tutor/ViewTutorResourceDetails').then(function (response) {
            $scope.ViewTutorResources = response.data;
            $scope.ViewTutorResourceDetails = new NgTableParams({}, { dataset: $scope.ViewTutorResources });
        });

    }


    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteInternTutorsResourceArray = function (index) {
        var d = $scope.arrInternTutorResourceFiles[index];
        var d1 = $scope.arrInternTutorMultipleResourceFiles[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrInternTutorResourceFiles.splice(index, 1);
        $scope.arrInternTutorMultipleResourceFiles.splice(index, 1);
    }


    // Delete Resource Resource By Dilshad A. on 03 Oct 2020
    $scope.DeleteInternTutorsResourceArray = function (index) {
        var d = $scope.arrInternTutorResourceFiles[index];
        var d1 = $scope.arrInternTutorMultipleResourceFiles[index];
        //$scope.deleteInternResourceArray.push({ InternshipId: data2.InternshipId, InternshipName: data2.InternshipName, ResourceId: data2.ResourceId, ResourceName: data2.ResourceName });
        $scope.arrInternTutorResourceFiles.splice(index, 1);
        $scope.arrInternTutorMultipleResourceFiles.splice(index, 1);
    }

    // Add Resource Intern Tutor Init

    $scope.addinterntutorresourceinit = function () {

        $http.get('/InternTutor/InternshipForInternTutor').then(function (response) {
            $scope.InternshipForTutor = response.data;
        })

    }

    // View Intern Tutor Init

    $scope.ViewInternTutorResourceDetails = function () {

        $http.get('/InternTutor/ViewInternTutorResourceDetails').then(function (response) {
            
            $scope.ViewInternTutorResources = response.data;
            $scope.ViewInternTutorResourceDetailsTable = new NgTableParams({}, { dataset: $scope.ViewInternTutorResources });
        });
    }


    $scope.Studentsettings = function () {
        $scope.hidebasic = true;
        $scope.hideprofilesetting = false;
    }

    $scope.Studentbasic = function () {
        $scope.hidebasic = false;
        $scope.hideprofilesetting = true;
    }


    $scope.AddMessageCommunicationForAll = function () {

      
        if ($scope.chkAllStudents == undefined || $scope.chkAllStudents == "") {
            $scope.chkAllStudents = false;
        }

        if ($scope.chkAllTutors == undefined || $scope.chkAllTutors == "") {
            $scope.chkAllTutors = false;
        }

        $http.post("/Admin/AddMessageCommunicationForAll",
            {                
                isAllStudents: $scope.chkAllStudents,
                isAllTutors: $scope.chkAllTutors,
                strMsgAll: $scope.mdlMsgAll
            }).then(function (resAddCommToAll) {
                if (resAddCommToAll.data > 0) {
                    alert("Message successfully sent.");
                    window.location.href = "/Admin/Communication";
                }
                else {
                    console.log(resAddComm.data);
                    alert("Failed. Something went wrong.");
                }
            });
    };


    $scope.AdminToBrandTutorIncomingComm = function () {

        $scope.usertype = "All-Brand-Tutor";
        $http.get('/Tutor/AdminToBrandTutorIncomingComm?strUserType= ' + $scope.usertype).then(function (response) {
            $scope.AdminToBrandTutorIncomingComm = response.data;
            $scope.AdminToBrandTutorIncomingMessages = new NgTableParams({}, { dataset: $scope.AdminToBrandTutorIncomingComm });
        })
    }

    // Add FAQ For Admin

    $scope.AddFAQ = function () {

        if (sessionStorage.FAQId !== undefined && sessionStorage.FAQId !== undefined) {
            $scope.data3 = JSON.parse(sessionStorage.FAQId);

            if ($scope.data3 !== undefined && $scope.data3 != '') {
                //$scope.FAQUpdate = {};
                //$scope.FAQUpdate.Question = $scope.FAQQuestion;
                //$scope.FAQUpdate.Answer = $scope.FAQAnswer;
                //$scope.FAQUpdate.QuestionType = $scope.FAQQuestionType;
                //$scope.FAQUpdate.IsPublished = $scope.IsPublishedForFAQ;
                //$scope.FAQUpdate.FAQId = $scope.data3.FAQId;

            }

            $http.post('/Admin/UpdateFAQ', { FAQId: $scope.data3.FAQId,Question: $scope.FAQQuestion, Answer: $scope.FAQAnswer, QuestionType: $scope.FAQQuestionType, IsPublished: $scope.IsPublishedForFAQ  }).then(function (response) {
                $scope.updatefaqdata = response.data;

                //if ($scope.updatefaqdata == true) {
                //    sessionStorage.clear();
                //    alert("FAQ updated successfully");
                //    location.href = "/Admin/FrequentlyAskedQuestions";
                //}
                //else {
                //    alert("Error while Updating FAQ");
                //}


                if (response.data == true) {
                    sessionStorage.clear();
                    alert("FAQ updated successfully");
                    location.href = "/Admin/FrequentlyAskedQuestions";
                }

                else if (response.data == false) {
                    sessionStorage.clear();
                    alert("FAQ already exists");
                }
                else {
                    alert("Error while Updating FAQ");
                }

            })
        }

        else {
        $scope.AddFrequentlyQuestions = {};
        $scope.AddFrequentlyQuestions.Question = $scope.FAQQuestion;
        $scope.AddFrequentlyQuestions.Answer = $scope.FAQAnswer;
        $scope.AddFrequentlyQuestions.QuestionType = $scope.FAQQuestionType;
        $scope.AddFrequentlyQuestions.IsPublished = $scope.IsPublishedForFAQ;

        $http.post('/Admin/AddFAQ', { objaddfaq: $scope.AddFrequentlyQuestions }).then(function (response) {
            $scope.AddFAQByAdmin = response.data;

            //if (response.data == true) {
            //    alert("FAQ added successfully");
            //    location.href = "/Admin/FrequentlyAskedQuestions";
            //}
            //else if (response.data == false) {
            //    alert("Error while adding FAQ");
            //}
            //else {
            //    alert("Error");
            //}


            if (response.data == true) {
                alert("FAQ  created successfully");
                location.href = "/Admin/FrequentlyAskedQuestions";
            }
            else if (response.data == false) {
                alert("FAQ already exists");
            }
            else if (response.data == "empty") {
                swal({
                    title: 'Error',
                    text: 'FAQ is mandatory',
                    icon: "warning",
                    dangerMode: "true"
                });

            }

            else {
                alert("Error! Problem while creating FAQ");
            }
        })
        }
    }


    // FAQ Init

    $scope.FAQinit = function () {

        $http.get('/Home/FAQByAdmin').then(function (response) {
            $scope.FAQByAdminToUserinitdata = response.data;
            $scope.ShowFAQToUserByAdmin = new NgTableParams({}, { dataset: $scope.FAQByAdminToUserinitdata });
        })
    }

    // FAQ Admin Init

    $scope.FAQAdmininit = function () {

        $http.get('/Admin/FAQinit').then(function (response) {
            $scope.FAQinitdata = response.data;
            $scope.ShowFAQ = new NgTableParams({}, { dataset: $scope.FAQinitdata });
        })
    }

    $scope.editFAQ = function (user) {

        $scope.FAQId = user;
        
        sessionStorage.FAQId = JSON.stringify($scope.FAQId);

        if (sessionStorage.FAQId !== undefined && sessionStorage.FAQId != '') {
            $scope.data2 = JSON.parse(sessionStorage.FAQId);
        }

        if ($scope.data2 !== undefined && $scope.data2 != '') {
            $scope.FAQQuestionType = $scope.data2.QuestionType;
            $scope.FAQAnswer = $scope.data2.Answer;
            $scope.IsPublishedForFAQ = $scope.data2.IsPublished;
            $scope.FAQQuestion = $scope.data2.Question;
        }
    }


    $scope.deleteFrequentlyAsked = function (Id) {
        $scope.Id = Id;
    }

    $scope.DeleteFAQ = function () {
        $http.post('/Admin/DeleteFAQ', { Id: $scope.Id }).then(function (response) {
            $scope.DeleteFAQ = response.data;

            if (response.data == true) {
                alert("FAQ deleted successfully");
                location.href = "/Admin/FrequentlyAskedQuestions";
            }
            else {
                alert("Error while deleting FAQ");
            }
        });
    };

   



    // Add User FAQ Details(If user FAQ question unable to get his answer from FAQ List) by Dilshad A. on 24 Nov 2020
    $scope.AddUserFAQDetails = function () {

        $scope.emailpattern = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;


        if ($scope.emailpattern.test($scope.mdlEmail)) {
        $scope.clsUserFAQ = {};
        $scope.clsUserFAQ.Name = $scope.mdlName;
        $scope.clsUserFAQ.Mobile = $scope.mdlMobile;
        $scope.clsUserFAQ.Email = $scope.mdlEmail;
        $scope.clsUserFAQ.Query = $scope.mdlQuery;
        $http.post("/Home/AddUserFAQDetails", { tblUserFAQ: $scope.clsUserFAQ }).then(function (res) {
            if (res.data > 0) {
                alert("You request has been added. I will update your question. Thanks for your query.");
                location.href = "/Home/FAQ"; 
                   }
            else {
                alert("Failed. Something went wrong. Try after some time!!!");
            }

            });
        }

    }

    $scope.SendOtpForFAQDetails = function () {

        $http.post('/Home/SendOtpForFAQDetails', { Name: $scope.mdlName, Email: $scope.mdlEmail, UserType: "FAQ" }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Mail Sent Successfully");
                $("#verifyEmailOtpFAQ").modal('show');
            }
            else {
                alert("Error while sending mail");
            }
        });

    }

    $scope.ConfirmFAQOtp = function () {
        $http.post('/Home/ConfirmEmailOtpForFAQ', { OTP: $scope.VerifyOTPForFAQ }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtpFAQ").modal('hide');
                $scope.AddUserFAQDetails();
            }
            else {
                alert("Error while verified OTP");
            }
        });
    }


    $scope.SendOtpForContact = function () {
        $http.post('/Home/SendOtpForContact', { Name: $scope.mdlContactName, Email: $scope.mdlContactEmail, UserType: "Contact" }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Mail Sent Successfully");
                $("#verifyEmailOtpCon").modal("show");
            }
            else {
                alert("Error while sending mail");
            }
        });

    }


    $scope.ConfirmContactOtp = function () {
        $http.post('/Home/ConfirmContactOtp', { OTP: $scope.VerifyOTPForContact }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtpCon").modal('hide');
                $scope.AddForContact();
            }
            else {
                alert("Error while verified OTP");
            }
        });
    }


    // Refer Friend Init

    $scope.referfriendinit = function () {
        $scope.emailregularex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        $scope.emailregularexp = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
        
    }
    


    $scope.SendOtpForRefer = function () {

        $http.post('/Home/SendOtpForRefer', { Name: $scope.mdlReferByName, Email: $scope.mdlReferByEmail, UserType: "Refer" }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("Mail Sent Successfully");
                $("#verifyEmailOtpRefer").modal('show');
            }
            else {
                alert("Error while sending mail");
            }
        });

    }


    $scope.ConfirmReferOtp = function () {
        $http.post('/Home/ConfirmReferOtp', { OTP: $scope.VerifyOTPForRefer }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtpRefer").modal('hide');
                $scope.AddReferFriend();
            }
            else {
                alert("Error while verified OTP");
            }
        });
    }



    // Add For Contact(Any Candidate can contant via this Name,Email and message) By Dilshad A. on 24 Nov 2020
    $scope.AddForContact = function () {
        $scope.clsContact = {};
        $scope.clsContact.Name = $scope.mdlContactName;
        $scope.clsContact.Email = $scope.mdlContactEmail;
        $scope.clsContact.Mobile = $scope.mdlContactMobile;
        $scope.clsContact.Country = $scope.mdlContactCountry;
        $scope.clsContact.Subject = $scope.mdlContactSubject;
        $scope.clsContact.Message = $scope.mdlContactMessage;

        $http.post("/Home/AddForContact", { tblContact: $scope.clsContact }).then(function (res) {
            if (res.data === "AllFieldsRequired") {
                alert("Failed. All fields are mandatory!!!");
                location.href = "/Home/Contact";
            }
            else if (res.data > 0) {
                alert("Your details has been recorded. Team will contact you soon. Thanks for contacting.");
                location.reload();
            }
            else {
                alert("Failed! Something went wrong. Try after some time!!!");
            }
        });
    };

    // Add ReferFriend by Dilshad A. on  25 Nov 2020
    $scope.AddReferFriend = function () {
        $scope.clsRefer = {};
        $scope.clsRefer.ReferBy = $scope.mdlReferByName;
        $scope.clsRefer.MobileRefereBy = $scope.mdlReferByMobile;
        $scope.clsRefer.EmailReferBy = $scope.mdlReferByEmail;
        $scope.clsRefer.ReferTo = $scope.mdlReferToName;
        $scope.clsRefer.MobileReferTo = $scope.mdlReferToMobile;
        $scope.clsRefer.EmailReferTo = $scope.mdlReferToEmail;
        $http.post("/Home/AddReferFriend", { objRefer: $scope.clsRefer}).then(function (res) {
            if (res.data > 0) {
                alert("Thanks for your reference. Team will contact you and refered person.");
                location.href = "/Home/ReferFriend";
            }
            else {
                alert("Failed. Something went wrong. Try in some time.");
            }
        });
    };


$scope.ViewUserFAQinit = function(){

$http.get('/Home/GetFAQDetail').then(function(response){

    $scope.FAQuserdata = response.data;
    $scope.ShowUserFAQ = new NgTableParams({}, { dataset: $scope.FAQuserdata });

});

    }

    // Contact Init

    $scope.Contactinit = function () {
        $scope.emailregularexpression = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    }

    $scope.StudentCategory = function () {
        $http.get('/User/StudentCategoryData').then(function (response) {
            $scope.StudentCategoriesData = response.data;
        })
    }

    $scope.Internships = function () {
    $http.get('/Internship/GetInternship').then(function (response) {
        $scope.InternshipDataBind = response.data;
    })
    }

    // Home Enquiry Init 
    $scope.Enquiryinit = function () {


        $scope.enquiryemailregex = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;

        $scope.hideInternship = true; 
        $scope.hideCategory = true; 
        $scope.hideCourse = true;
      //  $scope.hideCategory = true;
        $scope.StudentCategory()
        $scope.Internships()
       
    }


    $scope.getEnquiryType = function (type) {
        $scope.EnqType = type;

        if ($scope.EnqType == "Intern") {
            $scope.hideCategory = true;
            $scope.hideCourse = true;
            $scope.hideInternship = false;
        }

        else if ($scope.EnqType == "Course") {
            $scope.hideInternship = true;
            $scope.hideCategory = false;
            $scope.hideCourse = true;
        }

        else if ($scope.EnqType == "Other") {
            $scope.hideInternship = true;
            $scope.hideCategory = true;
            $scope.hideCourse = true;
        }

    }

    // Get Course Based On Category 

    $scope.getCourseForCategory = function (Id) {
        $scope.hideCourse = false;
        $http.post('/Home/GetCourseForCategoryEnquiry', { StudentCategoryId: Id }).then(function (response) {
            $scope.CourseForCategoryEnquiry = response.data;
        })
    }


    // Add Enquiry by Atul on 27 November

    $scope.AddEnquiry = function () {
        $scope.clsEnquiry = {};

        $scope.clsEnquiry.CandidateType = $scope.CandidateType;
        $scope.clsEnquiry.Name = $scope.Name;
        $scope.clsEnquiry.Mobile = $scope.Mobile;
        $scope.clsEnquiry.Email = $scope.Email;
        $scope.clsEnquiry.EnquiryType = $scope.EnquiryType;

        if ($scope.EnquiryType == "Intern") {
            $scope.clsEnquiry.InternshipId = $scope.intInternshipId;
        }

        else if ($scope.EnquiryType == "Course") {
            $scope.clsEnquiry.StudentCategoryId = $scope.StudentCategoryId;
            $scope.clsEnquiry.CourseId = $scope.intCourseId;
        }
        else if ($scope.EnquiryType == "Other") {
            $scope.clsEnquiry.EnquiryType = $scope.EnquiryType;
        }

        $scope.clsEnquiry.Message = $scope.Message; 

      //  $scope.clsEnquiry.InternshipId = $scope.intInternshipId;
              

       
        $http.post("/Home/AddEnquiry", { objEnquiry: $scope.clsEnquiry }).then(function (res) {
            if (res.data > 0) {
                alert("Thanks for your enquiry. Team will contact you soon.");
                location.href = "/Home/Enquiry";
            }
            else {
                alert("Failed. Something went wrong. Try in some time.");
            }
        });
    };


    // Send otp for Enquiry 

    $scope.SendOtpForEnquiry = function () {

        $http.post('/Home/SendOtpForEnquiry', { Name: $scope.Name, Email: $scope.Email, UserType: "Enquiry" }).then(function (response) {
            $scope.Enquirydata = response.data;
            if (response.data == true) {
                alert("Mail Sent Successfully");
                $("#verifyEmailOtpEnquiry").modal('show');
            }
            else {
                alert("Error while sending mail");
            }
        });

    }

    // Confirm Enquiry OTP

    $scope.ConfirmEnquiryOtp = function () {
        $http.post('/Home/ConfirmEnquiryOtp', { OTP: $scope.VerifyOTPForEnquiry }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtpEnquiry").modal('hide');
                $scope.AddEnquiry();
            }
            else {
                alert("Error while verified OTP");
            }
        });
    }


    // Get Refer Friend Details For Admin

    $scope.GetReferFriendDetails = function () {

        $http.get('/Admin/GetReferFriendDetails').then(function (response) {

            $scope.ReferFriendDetails = response.data;
            $scope.ReferFriendDetailsForAdmin = new NgTableParams({}, { dataset: $scope.ReferFriendDetails });

        })
    }


    // Get Contact Details For Admin 

    $scope.GetUserContactDetails = function () {
        $http.get('/Admin/GetUserContactDetails').then(function (response) {

            $scope.ContactDetails = response.data;
            $scope.ContactDetailsUserForAdmin = new NgTableParams({}, { dataset: $scope.ContactDetails });

        })
    }

    // Get Enquiry Details Of User For Admin

    $scope.GetUserEnquiryDetails = function () {
        $http.get('/Admin/GetUserEnquiryDetails').then(function (response) {

            $scope.EnquiryDetails = response.data;
            $scope.EnquiryDetailsUserForAdmin = new NgTableParams({}, { dataset: $scope.EnquiryDetails });

        })
    }



    

});
