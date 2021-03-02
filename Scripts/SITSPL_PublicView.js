var app = angular.module("app", ['ngFileUpload', 'ngTable', 'moment-picker', 'ngStorage', 'angucomplete-alt']);
app.controller("MyCtrl", function ($scope, $http, Upload, NgTableParams, $sessionStorage, $scope, $filter, $window) {

    //Get Course List By CourseName of all category by Dilshad A. on 14 Dec 2020
    $scope.GetAllCourseByCourseNameDetails = function (CourseId) {
        $http.get("/Home/GetAllCourseByCourseNameDetails?intCourseId=" + CourseId).then(function (res) {
            $scope.CourseByCourseNameSessionData = res.data;
            sessionStorage.setItem("CourseByCourseNameD", JSON.stringify($scope.CourseByCourseNameSessionData));
            window.location.href = "/Home/GetAllCourseByCourseName";
        });
    };

    //Get CourseList By CourseName of all category by Dilshad A.on 14 Dec 2020
    $scope.GetAllCourseByCourse = function () {
        var data = sessionStorage.getItem("CourseByCourseNameD");
        data = JSON.parse(data);
        $scope.CourseListByCourse = data;
    };

    // Get Course List Details By Dilshad A. on 14 Dec 2020
    $scope.getCoursesDetails = function (Id) {
        $scope.Id = Id;
        sessionStorage.CourseStructureId = JSON.stringify($scope.Id);
        location.href = "/User/CoursesListDetails";
    }

    $scope.getCourseStructureDetailsBasedOnId = function () {

        if (sessionStorage.CourseStructureId != undefined) {
            $scope.Id = JSON.parse(sessionStorage.CourseStructureId);
        }

        $http.get('/User/CourseStructureDataBasedOnId?Id=' + $scope.Id).then(function (response) {
            $scope.CourseBasedOnId = response.data;

            //Added By Dilshad on 15 Dec 2020
            sessionStorage.setItem("CourseStrucDetails", JSON.stringify($scope.CourseBasedOnId));
            $scope.data = JSON.parse($scope.CourseBasedOnId);
            // END Added By Dilshad on 15 Dec 2020

            $scope.DescriptionsContent = $scope.data.description;
            $scope.HeadingsContent = $scope.data.Heading;

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
                    $scope.descriptionarray.push($scope.data["description"][i]);
                }
            }

            // Storing Prerequisities for course
            $scope.prerequisitearray = [];
            $scope.data8 = [];

            if ($scope.data["prerequisities"] != '' && $scope.data["prerequisities"] != undefined) {
                for (var i = 0; i < $scope.data["prerequisities"].length; i++) {
                    $scope.prerequisitearray.push($scope.data["prerequisities"][i]);
                }
            }
        });
    };

    // Direct Apply on Course Details Page By Dilshad A. on 14 Dec 2020
    $scope.ShowApplyNowPop = function () {
        $("#popupApplyNow").modal("show");
    };

    //Send OTP for Course Registration of Student bby Dilshad A. on 14 Dec 2020
    $scope.SendOTPForRegisterCourse = function () {

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

    // Confirm OTP and Insert the tudent Details By Dilshad A. on  14 Dec 2020
    $scope.ConfirmOtpAndRegister = function () {
        $http.post('/User/ConfirmEmailOtp', { OTP: $scope.VerifyOTP }).then(function (response) {
            $scope.data = response.data;
            if (response.data == true) {
                alert("OTP verified successfully");
                $("#verifyEmailOtp").modal('hide');
                $scope.hidebasic = false;
                $scope.coursedetails = false;
                $scope.DisableOtpBtn = true;
                $scope.disableEmail = true;

                var dataForApplyFromCourseList = sessionStorage.getItem("CourseStrucDetails");
                var d = JSON.parse(dataForApplyFromCourseList);
                d = JSON.parse(d);
                if (d != null) {
                    $scope.clsCourseReg = {};
                    $scope.clsCourseReg.CourseId = d.struc[0].CourseId;
                    $scope.clsCourseReg.Duration = d.struc[0].DurationId;
                    $scope.clsCourseReg.CourseValidFrom = d.struc[0].VF;// moment(d.struc[0].ValidFrom);
                    $scope.clsCourseReg.CourseValidTo = d.struc[0].VT;//moment(d.struc[0].ValidTo);
                    $scope.clsCourseReg.Fees = d.struc[0].Fees;
                    $scope.clsCourseReg.FeesWithDiscount = d.struc[0].FeesWithDiscount;
                    $scope.clsCourseReg.DiscountPercent = d.struc[0].DiscountPercent;
                    $scope.clsCourseReg.NetAmountToPay = d.struc[0].NetAmount;
                    $scope.clsCourseReg.CreatedBy = $scope.Name;                    
                }

                $scope.StudentProfileReg = {};
                $scope.StudentProfileReg.Name = $scope.Name;
                $scope.StudentProfileReg.DOB = moment($scope.DOB).format("YYYY-MM-DD");
                $scope.StudentProfileReg.CreatedBy = $scope.Name;
                $scope.StudentProfileReg.Email = $scope.Email;
                $scope.StudentProfileReg.Mobile = $scope.Phone;
                $scope.StudentProfileReg.Username = $scope.Username;
                $scope.StudentProfileReg.Password = $scope.Password;
                $scope.StudentProfileReg.IsDeleted = false;
                $scope.StudentProfileReg.StudentCategoryId = 2;// 2 for UG $scope.StudentCategoryId;
                $scope.StudentProfileReg.BatchId = $scope.mdlBatchSize;
                $scope.StudentProfileReg.StudentType = "CourseStudent";

                Upload.upload({
                    url: "/User/AddStudentFromCourseList",
                    data: {
                        create: $scope.StudentProfileReg,
                        objCourseStr: $scope.clsCourseReg
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
                        $scope.AddActivity(parseInt(response.data), "Student", "Insert", "Student Msg", "Student Insert From Course List Page");
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
                alert("Problem while verifying OTP. Please try again!!!");
            }
        });
    };

    // Get BatchSize Details For Registration at the time of apply Course by Dilshad A. on 15 Dec 2020
    //$scope.GetBatchDetails = function () {
    //    $http.get('/User/BatchData').then(function (response) {            
    //        $scope.BatchDetails = response.data;
    //    });
    //};

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

    // GetSubHeadingByStudentCategoryId by Atul Sh. on 18 Dec 2020
    $scope.GetSubHeadingByStudentCategoryId = function () {
        $http.get("/Home/GetSubHeadingByStudentCategoryId").then(function (res) {
            $scope.SubHeadingByCatId = res.data;
        });
    };
});