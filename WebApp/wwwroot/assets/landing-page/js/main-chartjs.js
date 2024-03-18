$(document).ready(function () {

    var idGreenhouseGas = 'greenhouseGas'
    var greenhouseGasCO2MetricTonnesCO2e = $('#GreenhouseGasCO2MetricTonnesCO2e').val();
    var greenhouseGasCH4MetricTonnesCO2e = $('#GreenhouseGasCH4MetricTonnesCO2e').val();
    var greenhouseGasN2OMetricTonnesCO2e = $('#GreenhouseGasN2OMetricTonnesCO2e').val();
    var greenhouseGasPFCsMetricTonnesCO2e = $('#GreenhouseGasPFCsMetricTonnesCO2e').val();
    var greenhouseGasHFCsMetricTonnesCO2e = $('#GreenhouseGasHFCsMetricTonnesCO2e').val();
    var greenhouseGasSF6MetricTonnesCO2e = $('#GreenhouseGasSF6MetricTonnesCO2e').val();
    var greenhouseGasNF3MetricTonnesCO2e = $('#GreenhouseGasNF3MetricTonnesCO2e').val();
    greenhouseGasCO2MetricTonnesCO2e = greenhouseGasCO2MetricTonnesCO2e !== undefined && greenhouseGasCO2MetricTonnesCO2e !== '' ? parseFloat(greenhouseGasCO2MetricTonnesCO2e) : 0;
    greenhouseGasCH4MetricTonnesCO2e = greenhouseGasCH4MetricTonnesCO2e !== undefined && greenhouseGasCH4MetricTonnesCO2e !== '' ? parseFloat(greenhouseGasCH4MetricTonnesCO2e) : 0;
    greenhouseGasN2OMetricTonnesCO2e = greenhouseGasN2OMetricTonnesCO2e !== undefined && greenhouseGasN2OMetricTonnesCO2e !== '' ? parseFloat(greenhouseGasN2OMetricTonnesCO2e) : 0;
    greenhouseGasPFCsMetricTonnesCO2e = greenhouseGasPFCsMetricTonnesCO2e !== undefined && greenhouseGasPFCsMetricTonnesCO2e !== '' ? parseFloat(greenhouseGasPFCsMetricTonnesCO2e) : 0;
    greenhouseGasHFCsMetricTonnesCO2e = greenhouseGasHFCsMetricTonnesCO2e !== undefined && greenhouseGasHFCsMetricTonnesCO2e !== '' ? parseFloat(greenhouseGasHFCsMetricTonnesCO2e) : 0;
    greenhouseGasSF6MetricTonnesCO2e = greenhouseGasSF6MetricTonnesCO2e !== undefined && greenhouseGasSF6MetricTonnesCO2e !== '' ? parseFloat(greenhouseGasSF6MetricTonnesCO2e) : 0;
    greenhouseGasNF3MetricTonnesCO2e = greenhouseGasNF3MetricTonnesCO2e !== undefined && greenhouseGasNF3MetricTonnesCO2e !== '' ? parseFloat(greenhouseGasNF3MetricTonnesCO2e) : 0;
    var dataGreenhouseGas = [
        ["Carbon dioxide (CO2)", "Methane (CH4)", "Nitrous Oxide (N2O)", "Perfluorocarbons (PFCs)", "Hydrofluorocarbons (HFCs)", "Sulfur Hexafluoride (SF6)", "Nitrogen Trifluoride (NF3)"],
        [greenhouseGasCO2MetricTonnesCO2e, greenhouseGasCH4MetricTonnesCO2e, greenhouseGasN2OMetricTonnesCO2e, greenhouseGasPFCsMetricTonnesCO2e, greenhouseGasHFCsMetricTonnesCO2e, greenhouseGasSF6MetricTonnesCO2e, greenhouseGasNF3MetricTonnesCO2e]
    ]

    var idactivityScope1 = 'activityScope1';
    var idactivityScope2 = 'activityScope2';
    var idactivityScope3 = 'activityScope3';
    //get data from input have id=OnSiteCombustion
    var onSiteCombustion = $('#OnSiteCombustion').val();
    var onSiteVehicles = $('#OnSiteVehicles').val();
    var processFugitiveEmissions = $('#ProcessFugitiveEmissions').val();
    var inboundLogistics = $('#InboundLogistics').val();
    var outboundLogistics = $('#OutboundLogistics').val();
    var businessTravel = $('#BusinessTravel').val();
    // convert to decimal
    onSiteCombustion = onSiteCombustion !== undefined && onSiteCombustion !== '' ? parseFloat(onSiteCombustion) : 0;
    onSiteVehicles = onSiteVehicles !== undefined && onSiteVehicles !== '' ? parseFloat(onSiteVehicles) : 0;
    processFugitiveEmissions = processFugitiveEmissions !== undefined && processFugitiveEmissions !== '' ? parseFloat(processFugitiveEmissions) : 0;
    inboundLogistics = inboundLogistics !== undefined && inboundLogistics !== '' ? parseFloat(inboundLogistics) : 0;
    outboundLogistics = outboundLogistics !== undefined && outboundLogistics !== '' ? parseFloat(outboundLogistics) : 0;
    businessTravel = businessTravel !== undefined && businessTravel !== '' ? parseFloat(businessTravel) : 0;

    var dataActivityScope1 = [
        ["On-Site Combustion", "On-Site Vehicles", "Process and Fugitive Emissions", "Inbound Logistics", "Outbound Logistics", "Business Travel"],
        [onSiteCombustion, onSiteVehicles, processFugitiveEmissions, inboundLogistics, outboundLogistics, businessTravel]
    ]

    var electricity = $('#Electricity').val();
    var heat = $('#Heat').val();
    var steam = $('#Steam').val();
    var cooling = $('#Cooling').val();
    electricity = electricity !== undefined && electricity !== '' ? parseFloat(electricity) : 0;
    heat = heat !== undefined && heat !== '' ? parseFloat(heat) : 0;
    steam = steam !== undefined && steam !== '' ? parseFloat(steam) : 0;
    cooling = cooling !== undefined && cooling !== '' ? parseFloat(cooling) : 0;

    var dataActivityScope2 = [
        ["Electricity", "Heat", "Steam", "Cooling",],
        [electricity, heat, steam, cooling]
    ]

    var purchasedGoodsServices = $('#PurchasedGoodsServices').val();
    var fuelEnergyRelatedActivities = $('#FuelEnergyRelatedActivities').val();
    var upstreamTransportationDistribution = $('#UpstreamTransportationDistribution').val();
    var downstreamTransportationDistribution = $('#DownstreamTransportationDistribution').val();
    var wasteGeneratedOperations = $('#WasteGeneratedOperations').val();
    var businessTravel = $('#BusinessTravel').val();
    var employeeCommuting = $('#EmployeeCommuting').val();
    var scope3OnsiteTransport = $('#Scope3OnsiteTransport').val();
    purchasedGoodsServices = purchasedGoodsServices !== undefined && purchasedGoodsServices !== '' ? parseFloat(purchasedGoodsServices) : 0;
    fuelEnergyRelatedActivities = fuelEnergyRelatedActivities !== undefined && fuelEnergyRelatedActivities !== '' ? parseFloat(fuelEnergyRelatedActivities) : 0;
    upstreamTransportationDistribution = upstreamTransportationDistribution !== undefined && upstreamTransportationDistribution !== '' ? parseFloat(upstreamTransportationDistribution) : 0;
    upstreamTransportationDistribution = upstreamTransportationDistribution !== undefined && upstreamTransportationDistribution !== '' ? parseFloat(upstreamTransportationDistribution) : 0;
    downstreamTransportationDistribution = downstreamTransportationDistribution !== undefined && downstreamTransportationDistribution !== '' ? parseFloat(downstreamTransportationDistribution) : 0;
    wasteGeneratedOperations = wasteGeneratedOperations !== undefined && wasteGeneratedOperations !== '' ? parseFloat(wasteGeneratedOperations) : 0;
    businessTravel = businessTravel !== undefined && businessTravel !== '' ? parseFloat(businessTravel) : 0;
    employeeCommuting = employeeCommuting !== undefined && employeeCommuting !== '' ? parseFloat(employeeCommuting) : 0;
    scope3OnsiteTransport = scope3OnsiteTransport !== undefined && scope3OnsiteTransport !== '' ? parseFloat(scope3OnsiteTransport) : 0;

    var dataActivityScope3 = [
        ["Purchased goods & services", "Fuel- and energy-related activities", "Upstream transportation & distribution", "Downstream transportation & distribution", "Waste generated in operations", "Business travel", "Employee commuting", "Scope 3 onsite transport"],
        [purchasedGoodsServices, fuelEnergyRelatedActivities, upstreamTransportationDistribution, downstreamTransportationDistribution, wasteGeneratedOperations, businessTravel, employeeCommuting, scope3OnsiteTransport]
    ]

    var idEnergyScope3 = 'energyScope3';
    var enegyBreakdownFuel = $('#EnegyBreakdownFuel').val();
    var enegyBreakdownElectricity = $('#EnegyBreakdownElectricity').val();
    var enegyBreakdownHeat = $('#EnegyBreakdownHeat').val();
    var enegyBreakdownSteam = $('#EnegyBreakdownSteam').val();
    var enegyBreakdownCooling = $('#EnegyBreakdownCooling').val();
    enegyBreakdownFuel = enegyBreakdownFuel !== undefined && enegyBreakdownFuel !== '' ? parseFloat(enegyBreakdownFuel) : 0;
    enegyBreakdownElectricity = enegyBreakdownElectricity !== undefined && enegyBreakdownElectricity !== '' ? parseFloat(enegyBreakdownElectricity) : 0;
    enegyBreakdownHeat = enegyBreakdownHeat !== undefined && enegyBreakdownHeat !== '' ? parseFloat(enegyBreakdownHeat) : 0;
    enegyBreakdownSteam = enegyBreakdownSteam !== undefined && enegyBreakdownSteam !== '' ? parseFloat(enegyBreakdownSteam) : 0;
    enegyBreakdownCooling = enegyBreakdownCooling !== undefined && enegyBreakdownCooling !== '' ? parseFloat(enegyBreakdownCooling) : 0;
    var dataEnergyScope3 = [
        ["Fuel", "Electricity", "Heat", "Steam", "Cooling"],
        [enegyBreakdownFuel, enegyBreakdownElectricity, enegyBreakdownHeat, enegyBreakdownSteam, enegyBreakdownCooling]
    ]

    var idElectricityScope = 'electricityScope';
    var electricityGHCCO2 = $('#ElectricityGHCCO2').val();
    var electricityGHCCH4 = $('#ElectricityGHCCH4').val();
    var electricityGHCN2O = $('#ElectricityGHCN2O').val();
    var electricityGHCPFCs = $('#ElectricityGHCPFCs').val();
    var electricityGHCHFCs = $('#ElectricityGHCHFCs').val();
    var electricityGHCSF6 = $('#ElectricityGHCSF6').val();
    var electricityGHCNF3 = $('#ElectricityGHCNF3').val();
    electricityGHCCO2 = electricityGHCCO2 !== undefined && electricityGHCCO2 !== '' ? parseFloat(electricityGHCCO2) : 0;
    electricityGHCCH4 = electricityGHCCH4 !== undefined && electricityGHCCH4 !== '' ? parseFloat(electricityGHCCH4) : 0;
    electricityGHCN2O = electricityGHCN2O !== undefined && electricityGHCN2O !== '' ? parseFloat(electricityGHCN2O) : 0;
    electricityGHCPFCs = electricityGHCPFCs !== undefined && electricityGHCPFCs !== '' ? parseFloat(electricityGHCPFCs) : 0;
    electricityGHCHFCs = electricityGHCHFCs !== undefined && electricityGHCHFCs !== '' ? parseFloat(electricityGHCHFCs) : 0;
    electricityGHCSF6 = electricityGHCSF6 !== undefined && electricityGHCSF6 !== '' ? parseFloat(electricityGHCSF6) : 0;
    electricityGHCNF3 = electricityGHCNF3 !== undefined && electricityGHCNF3 !== '' ? parseFloat(electricityGHCNF3) : 0;
    var dataEelectricityScope = [
        ["Carbon dioxide (CO2)", "Methane (CH4)", "Nitrous Oxide (N2O)", "Perfluorocarbons (PFCs)", "Hydrofluorocarbons (HFCs)", "Sulfur Hexafluoride (SF6)", "Nitrogen Trifluoride (NF3)"],
        [electricityGHCCO2, electricityGHCCH4, electricityGHCN2O, electricityGHCPFCs, electricityGHCHFCs, electricityGHCSF6, electricityGHCNF3]
    ]


    var totalScope1 = $('#Scope1Total').val();
    var totalScope2 = $('#Scope2Total').val();
    var totalScope3 = $('#Scope3Total').val();
    totalScope1 = totalScope1 !== undefined && totalScope1 !== '' ? parseFloat(totalScope1) : 0;
    totalScope2 = totalScope2 !== undefined && totalScope2 !== '' ? parseFloat(totalScope2) : 0;
    totalScope3 = totalScope3 !== undefined && totalScope3 !== '' ? parseFloat(totalScope3) : 0;
    var idEmissions = 'emissionsChart';
    var dataEmissions = [
        ["Scope 1", "Scope 2", "Scope 3"],
        [{
            label: "Co2",
            data: [totalScope1, totalScope2, totalScope3],
            backgroundColor: '#CF1322'
        }]
    ]
    //var idcustomerChart = 'customerChart';
    //var dataCustomerChart = [
    //    ["Customer1", "Customer2", "Customer3"],
    //    [
    //        {
    //            label: "Scope 1",
    //            backgroundColor: "#4472c4",
    //            data: [3, 7, 4, 5, 7, 8],
    //        },
    //        {
    //            label: "Scope 2 ",
    //            backgroundColor: "#ed7d31",
    //            data: [3, 7, 4, 5, 7, 8],
    //        },
    //        {
    //            label: "Scope 3 Purchased goods & services ",
    //            backgroundColor: "#a5a5a5",
    //            data: [3, 7, 4, 5, 7, 8],
    //        },
    //        {
    //            label: "Scope 3 - Upstream transportation & distribution ",
    //            backgroundColor: "#FFC000",
    //            data: [3, 7, 4, 5, 7, 8],
    //        },
    //        {
    //            label: "Scope 3 Waste generated in operations",
    //            backgroundColor: "#22A121",
    //            data: [3, 7, 4, 5, 7, 8],
    //        },
    //        {
    //            label: "Scope 1, 2 and 3",
    //            backgroundColor: "#ED7D31",
    //            data: [3, 7, 4, 5, 7, 8],
    //        }
    //    ]
    //]

    const table = document.getElementById("customerData");
    // Lấy tất cả hàng từ bảng
    const rows = table.querySelectorAll("tbody tr");

    // Khởi tạo mảng cho mỗi loại dữ liệu
    let customerNames = [];
    let scope1Data = [];
    let scope2Data = [];
    let scope3PurchasedData = [];
    let scope3TransportationData = [];
    let scope3WasteData = [];
    let totalScopeData = [];

    // Lặp qua mỗi hàng để lấy dữ liệu
    rows.forEach(row => {
        const customerName = row.querySelector(".customer-name");
        const scope1 = row.querySelector(".scope1");
        const scope2 = row.querySelector(".scope2");
        const scope3Purchased = row.querySelector(".scope3-purchased");
        const scope3Transportation = row.querySelector(".scope3-transportation");
        const scope3Waste = row.querySelector(".scope3-waste");
        const totalScope = row.querySelector(".total-scope");

        customerNames.push(customerName ? customerName.innerText : '');

        scope1Data.push(scope1 ? parseFloat(scope1.innerText) || 0 : 0);
        scope2Data.push(scope2 ? parseFloat(scope2.innerText) || 0 : 0);
        scope3PurchasedData.push(scope3Purchased ? parseFloat(scope3Purchased.innerText) || 0 : 0);
        scope3TransportationData.push(scope3Transportation ? parseFloat(scope3Transportation.innerText) || 0 : 0);
        scope3WasteData.push(scope3Waste ? parseFloat(scope3Waste.innerText) || 0 : 0);
        totalScopeData.push(totalScope ? parseFloat(totalScope.innerText) || 0 : 0);
    });

    var idcustomerChart = 'customerChart';
    // Tạo biến cho biểu đồ
    var dataCustomerChart = [
        customerNames, // Tên khách hàng
        [
            { label: "Scope 1", backgroundColor: "#4472c4", data: scope1Data },
            { label: "Scope 2", backgroundColor: "#ed7d31", data: scope2Data },
            { label: "Scope 3 Purchased goods & services", backgroundColor: "#a5a5a5", data: scope3PurchasedData },
            { label: "Scope 3 - Upstream transportation & distribution", backgroundColor: "#FFC000", data: scope3TransportationData },
            { label: "Scope 3 Waste generated in operations", backgroundColor: "#22A121", data: scope3WasteData },
            { label: "Scope 1, 2 and 3", backgroundColor: "#ED7D31", data: totalScopeData }
        ]
    ];

    // Mảng để lưu trữ tên của các sản phẩm
    var productNames = [];
    // Mảng để lưu trữ giá trị CO2e cho mỗi sản phẩm
    var productCO2eValues = [];
    // Truy xuất tất cả các hàng sản phẩm
    var productRows = document.querySelectorAll(".tabcontent-body .product-row");
    productRows.forEach(function (row) {
        var productNameEl = row.querySelector("span"); // Lấy phần tử chứa tên sản phẩm
        var productName = productNameEl ? productNameEl.innerText.trim() : ""; // Kiểm tra và lấy tên sản phẩm, .trim() để loại bỏ khoảng trắng thừa
        if (productName) { // Kiểm tra để đảm bảo tên sản phẩm không phải là chuỗi rỗng
            productNames.push(productName);
        } else {
            productNames.push("Unknown Product"); // Hoặc xử lý tên sản phẩm không hợp lệ theo cách khác
        }

        var productCO2eEl = row.querySelector("strong"); // Lấy phần tử chứa giá trị CO2e
        var productCO2eStr = productCO2eEl ? productCO2eEl.innerText.trim() : ""; // Kiểm tra và lấy giá trị CO2e, .trim() để loại bỏ khoảng trắng thừa
        var productCO2e = parseFloat(productCO2eStr); // Chuyển đổi chuỗi thành số

        if (!isNaN(productCO2e)) { // Kiểm tra xem kết quả có là một số hợp lệ hay không
            productCO2eValues.push(productCO2e);
        } else {
            productCO2eValues.push(0); // Hoặc xử lý giá trị CO2e không hợp lệ theo cách khác
        }
    });
    var idProduct = 'productChart';

    var dataProduct = [
        productNames,
        [{
            label: "Co2",
            data: productCO2eValues,
            backgroundColor: '#CF1322'
        }]
    ]
    chartEmissions(dataProduct, idProduct);
    chartEmissions(dataEmissions, idEmissions);
    chartEmissions(dataCustomerChart, idcustomerChart);
    // chartPie(dataGreenhouseGas[0],dataGreenhouseGas[1],idGreenhouseGas);
    // chartPie(dataActivityScope1[0],dataActivityScope1[1],idactivityScope1);
    tabsElement();


    function chartEmissions(data, idElement) {
        var ctx = document.getElementById(idElement).getContext('2d');
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data[0],
                datasets: data[1],
            },
            options: {
                plugins: {
                    tooltip: {
                        enabled: true,
                    },
                    legend: {
                        display: true,
                        position: "bottom",
                        labels: {
                            usePointStyle: true,
                            boxWidth: 7,
                            boxHeight: 7,
                            padding: 20,
                        }
                    },
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        min: 0,
                        ticks: {
                            stepSize: 20,
                        }
                    },
                    x: {
                        barPercentage: 0.4,
                    }
                }
            }
        });
    }

    function chartPie(dataArr, idElement) {
        var ctx = document.getElementById(idElement).getContext('2d');
        ctx.width = 200;
        ctx.height = 200;
        var chart = new Chart(ctx, {
            type: "pie",
            data: {
                labels: dataArr[0],
                datasets: [
                    {
                        backgroundColor: ["#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5", "#F5222D", "#22A121"],
                        data: dataArr[1]
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        maxWidth: 100,
                        position: "bottom",
                        labels: {
                            usePointStyle: true,
                            boxWidth: 7,
                            padding: 15
                        }
                    },
                    tooltip: {
                        backgroundColor: "rgba(255, 255, 255, 1)",
                        titleColor: "rgba(44, 44, 44, 1)",
                        bodyColor: "rgba(125, 125, 125, 1)",
                        borderColor: "#9aa1ab26",
                        borderWidth: 1
                    },
                    datalabels: {
                        color: "white",
                        textAlign: "center",
                        display: true, // Set to true to display the labels
                        formatter: function (value, context) {
                            var dataset = context.chart.data.datasets[context.datasetIndex];
                            var total = dataset.data.reduce(function (previousValue, currentValue) {
                                return previousValue + currentValue;
                            });
                            var currentValue = dataset.data[context.dataIndex];
                            var percentage = ((currentValue / total) * 100).toFixed(2);
                            return percentage + "%";
                        },
                        color: '#fff',
                    }
                }
            },
            plugins: [ChartDataLabels]
        });
    }


    function tabsElement() {
        // Mặc định, ẩn tất cả nội dung tab ngoại trừ tab đầu tiên
        $(".tabcontent").not(":first").hide();

        // Xử lý sự kiện khi click vào tab
        $(".tablinks").on("click", function () {
            var tabId = $(this).data("tab");
            // Ẩn tất cả nội dung tab
            $(".tabcontent").hide();

            $("#" + tabId).show();
            checkTabsChart(tabId)


            // Đánh dấu tablink được chọn
            $(".tablinks").removeClass("active");
            $(this).addClass("active");
        });
    }

    var chartPieInitialized = {
        "Tab2": false,
        "Tab3": false,
        "Tab4": false,
        "Tab5": false,
        // Thêm các tab khác nếu cần
    };

    function checkTabsChart(tabId) {
        if (!chartPieInitialized[tabId]) {
            if (tabId === "Tab2") {
                chartPie(dataGreenhouseGas, idGreenhouseGas);
                chartPieInitialized[tabId] = true; // Đánh dấu rằng biểu đồ đã được tạo cho tab này
            }
            if (tabId === "Tab3") {
                chartPie(dataActivityScope1, idactivityScope1);
                chartPie(dataActivityScope2, idactivityScope2);
                chartPie(dataActivityScope3, idactivityScope3);
                chartPieInitialized[tabId] = true; // Đánh dấu rằng biểu đồ đã được tạo cho tab này
            }
            if (tabId === "Tab4") {
                chartPie(dataEnergyScope3, idEnergyScope3);
                chartPieInitialized[tabId] = true; // Đánh dấu rằng biểu đồ đã được tạo cho tab này
            }
            if (tabId === "Tab5") {
                chartPie(dataEelectricityScope, idElectricityScope);
                chartPieInitialized[tabId] = true; // Đánh dấu rằng biểu đồ đã được tạo cho tab này
            }
        }
    }
    function destroyChart(id) {
        var canvas = document.getElementById(id);
        Chart.getChart(canvas).destroy();
    }
    function getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
    if ($('.print-container').length) {

        chartPie(dataGreenhouseGas, idGreenhouseGas);

        chartPie(dataActivityScope1, idactivityScope1);
        chartPie(dataActivityScope2, idactivityScope2);
        chartPie(dataActivityScope3, idactivityScope3);

        chartPie(dataEnergyScope3, idEnergyScope3);

        chartPie(dataEelectricityScope, idElectricityScope);

    }
})