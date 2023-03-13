
/** Обновление графиков / диаграмм (вызывать первым, когда нужно обновление данных) */
export function updateCharts() {
    Chart.helpers.each(Chart.instances, function (instance) {
        instance.destroy();
    });
}

/** Добавление данных в диаграмму */
function addData(chart, label, data) {
    chart.data.labels.push(label);
    chart.data.datasets.forEach((dataset) => {
        dataset.data.push(data);
    });
    chart.update();
}

/** Получение случайного тёмного цвета */
function randDarkColor() {
    var lum = -0.25;
    var hex = String('#' + Math.random().toString(16).slice(2, 8).toUpperCase()).replace(/[^0-9a-f]/gi, '');
    if (hex.length < 6) {
        hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
    }
    var rgb = "#",
        c, i;
    for (i = 0; i < 3; i++) {
        c = parseInt(hex.substr(i * 2, 2), 16);
        c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
        rgb += ("00" + c).substr(c.length);
    }
    return rgb;
}

/** Получение цвета для датасета бункера/силоса (расход по бункеру) */
function bunkerDatasetColor(label) {
    // Получение цвета для датасета бункера/силоса по его номеру
    var color = {
        '1': "#ff0060",
        '2': "#33a961",
        '3': "#000194",
        '4': "#008080",
        '5': "#ff0000",
        '6': "#e6e6fa",
        '7': "#ffd700",
        '8': "#ffa500",
        '9': "#00ffff",
        '10': "#ff7373",
        '11': "#40e0d0",
        '12': "#0000ff",
        '13': "#f0f8ff",
        '14': "#d3ffce",
        '15': "#b0e0e6",
        '16': "#c6e2ff",
        '17': "#666666",
        '18': "#faebd7",
        '19': "#bada55",
        '20': "#003366",
        '21': "#ffb6c1",
        '22': "#fa8072",
        '23': "#ffff00",
        '24': "#00ff00",
        '25': "#ff00ff",
        '26': "#000080",
        '27': "#00ff7f",
        '28': "#a0db8e",
        '29': "#81d8d0",
        '30': "#0e2f44",
    }

    return color[label];
}

/** Диаграмма цветов компонентов в рецепте*/
export function componentColorChart(dataset, components, labels) {
    var chart = document.getElementById("components-donut").getContext("2d");

    var amounts = dataset.map(function (el) { return el.amount });
    var componentColors = [];

    for (let i = 0; i < labels.length; i++)
        componentColors.push(components.filter(w => w.title == labels[i])[0].color);

    const data = {
        labels: labels,
        datasets: [{
            data: amounts,
            backgroundColor: componentColors,
            hoverOffset: 8
        }]
    };

    const config = {
        type: 'doughnut',
        data: data,
        plugins: [ChartDataLabels],
        options: {
            responsive: true,
            plugins: {
                datalabels: {
                    align: 'center',
                    formatter: (value, ctx) => {
                        const dataPoints = ctx.chart.data.datasets[0].data;
                        function totalSum(total, dataPoint) {
                            return total + dataPoint;
                        }
                        const totalValue = dataPoints.reduce(totalSum, 0);
                        const percentageValue = (value / totalValue * 100).toFixed(1);
                        const display = [`${percentageValue}%`];
                        return display;
                    },
                    color: '#fff',
                    font: {
                        size: 16
                    }
                },
                legend: {
                    display: false,
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                },
                title: {
                    text: ''
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );

    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** График консистенций по конусу */
export function consistencyRelativeMixerPowerByConus(series, labels, colors) {
    /**
    * Документация для линейной диаграммы
    * https://www.chartjs.org/docs/3.7.1/samples/line/line.html
    */

    var chart = document.getElementById("consistency-chart").getContext("2d");

    var dataPoints = [];
    var step = 5;

    for (let i = 0; i < series.length; i++) {
        dataPoints.push({
            x: series[i] * 0.5, y: series[i]
        });
    }

    const data = {
        labels: labels,
        datasets: [
            {
                label: '',
                data: dataPoints,
                borderColor: 'black',
                backgroundColor: ['red'],
                pointRadius: 8
            }
        ],
    };

    const config = {
        type: 'line',
        data: data,
        options: {
            scaleShowHorizontalLines: true,
            scaleShowVerticalLines: true,
            maintainAspectRatio: false,
            pointDotRadius: 24,
            plugins: {
                title: {
                    display: false
                },
                legend: {
                    display: false
                }
            },
            scales: {
                x: {
                    display: false,
                    type: 'linear',
                    beginAtZero: true,
                    max: 30,
                    ticks: {
                        stepSize: step,
                    },
                    position: 'bottom',
                    grid: {
                        color: 'red',
                        borderColor: 'grey',
                        tickColor: 'grey'
                    }
                },
                y: {
                    type: 'linear',
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        stepSize: step,
                    },
                    position: 'left',
                    stack: 'demo',
                    stackWeight: 2,
                    grid: {
                        borderColor: ''
                    }
                },
                y2: {
                    type: 'linear',
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        stepSize: step,
                    },
                    position: 'right',
                    stack: 'demo',
                    stackWeight: 1,
                    grid: {
                        borderColor: ''
                    }
                }
            }
        },
    };

    var myChart = new Chart(
        chart,
        config
    );
}

/** График консистенций по марке */
export function consistencyRelativeMixerPowerByBrand(series, labels, colors) {
    /**
    * Документация для линейной диаграммы
    * https://www.chartjs.org/docs/3.7.1/samples/line/line.html
    */

    var chart = document.getElementById("consistency-chart").getContext("2d");

    var barColors = [];
    var barData = [];

    for (let i = 0; i < series.length; i++) {
        barColors.push(`rgb(${colors[i]})`);
        barData.push(100);
    }

    const data = {
        labels: labels,
        datasets: [
            {
                type: 'line',
                fill: false,
                label: '',
                borderColor: 'black',
                backgroundColor: ['red'],
                pointRadius: 8,
                max: 100,
                data: series
            },
            {
                type: 'bar',
                label: '',
                backgroundColor: barColors,
                beginAtZero: true,
                max: 100,
                data: barData
            }
        ]
    };

    const config = {
        data: data,
        options: {
            scaleShowHorizontalLines: true,
            scaleShowVerticalLines: true,
            maintainAspectRatio: false,
            pointDotRadius: 24,
            plugins: {
                title: {
                    display: false
                },
                legend: {
                    display: false
                },
                tooltip: {
                    enabled: true
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );
}

/** Диаграмма производительности на замес */
export function performanceOnBatch(dataset, dataset1, batchNumbers, dataSetLabel1, dataSetLabel2) {
    /**
     * Документация для столбчатой диаграммы
     * https://www.chartjs.org/docs/latest/charts/bar.html
     */
    if (!document.getElementById("batch-average-chart")) {
        return;
    }
        
    var chart = document.getElementById("batch-average-chart").getContext("2d");

    const data = {
        labels: batchNumbers,
        datasets: [
            {
                label: dataSetLabel1,
                data: dataset,
                borderColor: '#4265ED',
                backgroundColor: '#4265ED'
            },
            {
                label: dataSetLabel2,
                data: dataset1,
                borderColor: '#15B977',
                backgroundColor: '#15B977'
            }
        ]
    };

    const config = {
        type: 'bar',
        data: data,
        options: {
            maintainAspectRatio: false,
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        display: false
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                    position: 'top',
                },
                datalabels: {
                    offset: 5,
                    anchor: 'end',
                    align: 'end',
                    color: 'black'
                },
                title: {
                    display: false,
                    text: ''
                },
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'xy',
                        threshold: 10
                    },
                    zoom: {
                        enabled: true,
                        wheel: {
                            enabled: true,
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'xy',
                    }
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );


    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** Диаграмма точности дозатора */
export function dosageAccuracyBarChart(dataset, labels, categories, components) {
    /**
    * Документация для столбчатой диаграммы
    * https://www.chartjs.org/docs/latest/charts/bar.html
    */

    var chart = document.getElementById("myChart").getContext("2d");

    const data = {
        labels: categories,
        datasets: []
    };

    labels.forEach(function (item, i, labels) {
        let componentColor = components.filter(c => c.title == item)[0].color;
        data.datasets.push({ label: item, data: dataset[i], backgroundColor: componentColor });
    });

    const config = {
        type: 'bar',
        data: data,
        options: {
            responsive: true,
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    grid: {
                        display: false
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                    position: 'top',
                },
                title: {
                    display: false,
                    text: ''
                },
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'xy',
                        threshold: 10
                    },
                    zoom: {
                        wheel: {
                            enabled: true,
                            speed: 0.5
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'xy'
                    }
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );

    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** Диаграмма заказанного/выполненного объёма */
export function orderedVolume(dataset1, dataset2, dates, labels) {
    /**
    * Документация для столбчатой диаграммы
    * https://www.chartjs.org/docs/latest/charts/bar.html
    */

    var chart = document.getElementById("ordered-volume-chart").getContext("2d");

    const data = {
        labels: dates,
        datasets: [
            {
                label: labels[0],
                data: dataset1,
                backgroundColor: 'blue',
            },
            {
                label: labels[1],
                data: dataset2,
                backgroundColor: 'green',
            }
        ]
    };

    const config = {
        type: 'bar',
        data: data,
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        reverse: false,
                        stepSize: 3
                    },
                }]
            },
            pan: {
                enabled: true,
                mode: 'x',
            },
            zoom: {
                enabled: true,
                mode: 'x',
            },
        },
    };

    var myChart = new Chart(
        chart,
        config
    );

    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** Диаграмма средней фактической/потенциальной по замесам*/
export function factPotentialBarChart(dataset1, dataset2, dates, labels) {
    /**
    * Документация для столбчатой диаграммы
    * https://www.chartjs.org/docs/latest/charts/bar.html
    */

    var chart = document.getElementById("fact-potential-chart").getContext("2d");

    const data = {
        labels: dates,
        datasets: [
            {
                label: labels[0],
                data: dataset1,
                backgroundColor: 'yellow',
            },
            {
                label: labels[1],
                data: dataset2,
                backgroundColor: 'green',
            }
        ]
    };

    const config = {
        type: 'bar',
        data: data,
        options: {
            responsive: true,
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    grid: {
                        display: false
                    }
                }
            },
            events: ['mousemove', 'mouseout', 'click', 'touchstart', 'touchmove'],
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                    position: 'top',
                },
                scales: {
                    scaleLabel: {
                        display: true,
                        fontSize: 30,
                        fontColor: "#4a4a4a"
                    },
                    yAxes: [{
                        ticks: {
                            fontSize: 30,
                            stepSize: 1,
                        }
                    }]
                },
                title: {
                    display: false,
                    text: ''
                },
                zoom: {
                    zoom: {
                        wheel: {
                            enabled: true,
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'xy',
                    },
                    pan: {
                        enabled: true,
                        mode: 'xy',
                    },
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );

    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** Диаграмма расходов */
export function expendituresBarChart(dataset, labels, categories) {
    /**
    * Документация для столбчатой диаграммы
    * https://www.chartjs.org/docs/latest/charts/bar.html
    */

    var chart = document.getElementById("expenditures-chart").getContext("2d");
    var label = labels;

    const data = {
        labels: label,
        datasets: []
    };

    categories.forEach(function (item, i, labels) {
        let temp = dataset[item];
        let expenditures = temp.map(function (el) { return el.expenditures });
        data.datasets.push({ label: `Силос ${labels[i]}`, data: expenditures, backgroundColor: bunkerDatasetColor(item), stepped: true, backgroundColor: 'red', fill: false, borderColor: 'red' });
    });

    const config = {
        type: 'line',
        data: data,
        options: {
            maintainAspectRatio: false,
            responsive: true,
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                    position: 'top',
                },
                interaction: {
                    intersect: false,
                    axis: 'x'
                },
                plugins: {
                    title: {
                        display: true,
                        text: (ctx) => 'Step ' + ctx.chart.data.datasets[0].stepped + ' Interpolation',
                    }
                },
                //title: {
                //    display: true,
                //    text: 'График расходов',
                //    font: {
                //        size: 20
                //    }
                //},
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'xy',
                        //  overScaleMode: 'xy',
                        threshold: 10
                    },
                    zoom: {
                        wheel: {
                            enabled: true,
                            speed: 0.5
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'xy'
                    }
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );

    window.addEventListener('beforeprint', () => {
        myChart.resize(600, 600);
    });
}

/** Диаграмма остатков */
export function remainsBarChart(dataset, labels, categories) {
    /**
    * Документация для линейной диаграммы
    * https://www.chartjs.org/docs/3.6.2/samples/line/line.html
    */

    var chart = document.getElementById("remains-chart").getContext("2d");
    var label = labels;
    var dates = [];

    const data = {
        labels: label,
        datasets: []
    };

    categories.forEach(function (item, i, labels) {
        let temp = dataset[item];

        data.datasets.push({ label: `Силос ${labels[i]}`, data: [], borderColor: bunkerDatasetColor(item), fill: false, stepped: false });

        for (let index = 0; index < temp.length; index++)
            data.datasets[i].data.push({ x: temp[index].date, y: temp[index].remains });
    });

    const config = {
        type: 'line',
        data: data,
        options: {
            maintainAspectRatio: false,
            responsive: true,
            spanGaps: true,
            scales: {
                x: {
                    ticks: {
                        maxTicksLimit: dates.length,
                        callback: function (value, index, ticks) {
                            return index % 2 === 0 ? this.getLabelForValue(value).split(' ')[0] : ''
                        }
                    },
                },
                y: {
                    title: {
                        display: true,
                        text: 'Остаток'
                    }
                }
            },
            elements: {
                point: {
                    radius: 0,
                    hoverRadius: 8
                }
            },
            interaction: {
                intersect: false,
                axis: 'xy'
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 20
                        }
                    },
                    position: 'top'
                },
                title: {
                    display: true,
                    text: 'График остатков',
                    font: {
                        size: 20
                    }
                },
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'xy',
                        threshold: 10
                    },
                    zoom: {
                        wheel: {
                            enabled: true,
                            speed: 0.5
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'xy'
                    }
                }
            }
        }
    };

    var myChart = new Chart(
        chart,
        config
    );

    myChart.update();

    window.addEventListener('afterprint', () => {
        myChart.resize();
    });
}

/** Диаграммы Гантта - циклограмма */
export function highchartXRangeGantt(data, categories) {
    var categoriesWithSubcategories = [];
    var subcategories = [];
    var dataset = [];

    const color = {};
    var transparent = '#FFFFFF';

    console.log(data);

    if (categories.length >= 4 && categories[3][0].mechanicName == 'Смеситель_1' && categories[4][0].mechanicName == 'Скип') {
        let mixer = categories[3];
        let skip = categories[4];

        // Меняем местами скип со смесителем
        categories[3] = skip;
        categories[4] = mixer;
    }

    let categoryIndex = 0;
    for (let i = 0; i < categories.length; i++) {
        let mechanic = categories[i].map(function (el) { return el.mechanicName })[0];
        let phases = categories[i].filter(c => c.mechanicName == mechanic);

        //Избавляемся от дублирования фаз механики 
        phases = [...new Set(phases.map(function (el) { return el.name }))];

        let phaseIndex = 0;
        for (let phase of phases) {
            // Добавляем родительскую и дочернюю категорию, чтобы данные появились в дочерней 
            if (phaseIndex == 0) {
                categoriesWithSubcategories.push(mechanic);
                categoriesWithSubcategories.push(phase);
            }

            if (phaseIndex != 0)
                categoriesWithSubcategories.push(phase);

            // Увеличиваем индекс на единицу для отоброжения пустых линий механизмов (родительских категорий)    
            if (phaseIndex == phases.length || phaseIndex == 0)
                categoryIndex++;

            subcategories.push({ phase: phase, mechanic: mechanic, index: categoryIndex });
            color[mechanic] = randDarkColor();

            categoryIndex++;
            phaseIndex++;
        }

        for (let j = 0; j < categories[i].length; j++) {
            let start = new Date(categories[i][j].start).getTime();
            let end = new Date(categories[i][j].end).getTime();
            let mechanicName = categories[i][j].mechanicName;
            let phase = categories[i][j].name;
            let description = categories[i][j].description;
            let totalSeconds = categories[i][j].totalSeconds;

            let yAxisIndex = subcategories.filter(w => w.mechanic == mechanicName && w.phase == phase)[0].index;

            if (phase == 'Ожидание')
                dataset.push({
                    x: start, x2: end, y: yAxisIndex, phase: phase, mechanicName: mechanicName,
                    description: description, totalSeconds: totalSeconds, color: transparent, borderColor: 'black',
                    dataLabels: { enabled: true, color: 'black' }
                });
            else {
                let batchColor = categories[i][j].batchColor;
                dataset.push({
                    x: start, x2: end, y: yAxisIndex, phase: phase, mechanicName: mechanicName,
                    description: description, totalSeconds: totalSeconds, color: batchColor
                });
            }
        }
    }


    Highcharts.setOptions({
        global: {
            useUTC: false
        },

        lang: {
            downloadJPEG: 'Загрузить JPEG изображение',
            downloadPDF: 'Загрузить PDF документ',
            downloadPNG: 'Загрузить PNG изображение',
            downloadSVG: 'Загрузить SVG изображение',
            viewFullscreen: 'Cмотреть в полноэкранном режиме',
            exitFullscreen: 'Выйти из полноэкранного режима',
            printChart: 'Распечатать циклограмму',
            months: [
                'Янв.', 'Фев.', 'Март', 'Апр.',
                'Май', 'Июнь', 'Июль', 'Авг.',
                'Сен.', 'Окт.', 'Ноя.', 'Дек.'
            ],
            shortMonths: [
                'янв.', 'фев.', 'март', 'апр.',
                'мая', 'июня', 'июля', 'авг.',
                'сен.', 'окт.', 'ноя.', 'дек.'
            ],
            resetZoom: 'Сбросить масштабирование'
        }
    });

    var ganttChart = Highcharts.chart('container', {
        plotOptions: {
            series: {
                // general options for all series
                //boostThreshold: 1000, 
                turboThreshold: 0,
                borderWidth: 233,
                animation: true,
                dataLabels: {
                    enabled: true,
                    backgroundColor: 'rgba(255,255,255, 0.1)',
                    color: '#FFFFFF',
                    style: {
                        fontSize: '15px',
                        maxWidth: '10px',
                        textOverflow: "ellipsis",
                        overflow: "hidden"
                    },
                    useHTML: true,
                    inside: true,
                    formatter: function () {
                        if (this.point.phase == 'Ожидание')
                            return this.point.totalSeconds <= 5 ? "" : `${this.point.totalSeconds}`;

                        return this.point.totalSeconds <= 5 ? "" : this.point.totalSeconds + " "
                    }
                },
            },
            xrange: {
                // shared options for all xrange series
                animation: false,
                groupPadding: 10,
                pointWidth: 30,
                pointPadding: 1000,
                grouping: true,
                showInLegend: true
            },
        },
        boost: {
            // seriesThreshold: 50,
            // usePreallocated: true,
        },
        rangeSelector: {
            inputPosition: {
                align: 'left',
                x: 0,
                y: 0
            },
            buttonPosition: {
                align: 'right',
                x: 0,
                y: 0
            },
        },

        chart: {
            animation: false,
            type: 'xrange',
            zoomType: 'x',
            panKey: 'shift',
            panning: {
                enabled: true,
                type: 'x',
            },
            loading: {
                hideDuration: 100,
                showDuration: 100
            },
            events: {
                load: function () {
                    //   this.series.forEach(function(series) {
                    //     var points = series.points
                    //     points.forEach(function(point) {
                    //       if (point.shapeArgs.width < point.dataLabel.width) {
                    //         point.dataLabel.hide();
                    //       }
                    //     });
                    //   });

                    //--------------------------------------

                    //   var categoryWidth = 0.7,
                    //   points = this.series[0].points;

                    //   this.update({
                    //     chart: {
                    //         scrollablePlotArea: {
                    //             minHeight: this.chartHeight - this.plotHeight + categoryWidth * points.length
                    //         }
                    //     }
                    //   }, true, true, false);
                }
            },
        },
        tooltip: {
            animation: false,
            outside: true,
            borderWidth: 200,
            formatter: function () {
                let from = new Date(this.point.x).toLocaleString(),
                    to = new Date(this.point.x2).toLocaleString(),
                    description = this.point.description,
                    totalSeconds = this.point.totalSeconds,
                    mechanic = this.point.mechanicName,
                    phase = this.point.phase;

                return ` ${description}
                        <br>
                        Механика: ${mechanic}
                        <br>
                        Фаза: ${phase}
                        <br>
                        Длительность: ${totalSeconds} cек.
                        <br>
                        Начат: ${from}
                        <br>
                        Завершён: ${to}
                        `;
            },
            style: {
                color: '#000',
                fontSize: '20px',
                font: 'Trebuchet MS, Verdana, sans-serif'
            }
        },
        title: {
            text: ''
        },
        xAxis: {
            type: 'datetime',
            max: new Date(categories[categories.length - 1][(categories[0].length - 1) % 10].end).getTime(),
            min: new Date(categories[0][0].start).getTime(),
            labels: {
                formatter: function () {
                    return Highcharts.dateFormat('%e. %b %H:%M:%S',
                        this.value);
                },
                style: {
                    color: '#000',
                    font: '16px Trebuchet MS, Verdana, sans-serif'
                }
            },
            scrollbar: {
                height: 30,
                minWidth: 30,
                enabled: true,
                liveRedraw: false
            },
        },
        yAxis: {
            title: {
                text: ''
            },
            labels: {
                formatter() {
                    // Закрашиваем родительские категории (механизмы)
                    if (color.hasOwnProperty(this.value))
                        return `<p style="font-size:25px; color: ${color[this.value]}">${this.value}</p>`
                    // Обычный цвет дочерних категорий    
                    else
                        return `<p>${this.value}</p>`
                },
                style: {
                    color: '#000',
                    font: '18px Trebuchet MS, Verdana, sans-serif'
                }
            },
            categories: categoriesWithSubcategories,
            reversed: true
        },

        series: [{
            name: 'Механизмы',
            //borderColor: 'black',
            data: dataset,
            dataGrouping: {
                enabled: true,
                forced: true,
                groupAll: true,
            }
        }]
    });
}