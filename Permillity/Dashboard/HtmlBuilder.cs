using System.Text;

namespace Permillity.Dashboard
{
    internal static class HtmlBuilder
    {
        public static string GetDashboardHtml(string serializedData)
        {
            var sb = new StringBuilder();

            sb.Append(@"<!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Permillity Dashboard</title>");

            sb.Append(GetHtmlStyling());

            sb.Append(@"</head>
            <body>
                <h1>Permillity Dashboard</h1>
                <div class=""search-container"">
                    <input type=""text"" id=""search"" placeholder=""Search by Path or Method"">
                </div>
                <table id=""dashboardTable"">
                    <thead>
                        <tr>
                            <th class=""sortable"" data-column=""RequestMethod"">Method<span class=""sort-indicator""></span></th>
                            <th class=""sortable"" data-column=""RequestPath"">Path<span class=""sort-indicator""></span></th>
                            <th class=""sortable"" data-column=""RequestCount"">Count<span class=""sort-indicator""></span></th>
                            <th class=""sortable"" data-column=""AverageTime"">Average Time (ms)<span class=""sort-indicator""></span></th>
                            <th class=""sortable"" data-column=""TotalTime"">Total Time (s)<span class=""sort-indicator""></span></th>
                            <th class=""sortable"" data-column=""MaximumTime"">Max Time (ms)<span class=""sort-indicator""></span></th>
                            <th>Example Query Path</th>
                            <th>Example Body</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody id=""tableBody"">
                        <!-- Dynamic Rows Here -->
                    </tbody>
                    <tfoot>
                        <tr class=""total-row"" id=""totalsRow"">
                            <!-- Totals calculated dynamically -->
                        </tr>
                    </tfoot>
                </table>");

            sb.Append(GetHtmlScript(serializedData));

            sb.Append(@"</body></html>");

            return sb.ToString();
        }

        private static string GetHtmlScript(string serializedData)
        {
            return $"<script> const dashboardData = {serializedData};" +
                @"let sortedColumn = ""TotalTime"";
            let sortDirection = ""desc""; // Default sort by TotalTime descending

            const tableBody = document.getElementById(""tableBody"");
            const totalsRow = document.getElementById(""totalsRow"");

            function renderTable(data) {
                tableBody.innerHTML = """";

                let totalRequestCount = 0;
                let totalAverageTime = 0;
                let totalTotalTime = 0;
                let totalMaxTime = 0;

                data.forEach((row, index) => {
                    totalRequestCount += row.RequestCount;
                    totalAverageTime += row.AverageTime * row.RequestCount;
                    totalTotalTime += row.TotalTime;
                    totalMaxTime = Math.max(totalMaxTime, row.MaximumTime);

                    const mainRow = document.createElement(""tr"");
                    mainRow.innerHTML = `
                        <td>${row.RequestMethod}</td>
                        <td>${row.RequestPath}</td>
                        <td>${row.RequestCount}</td>
                        <td>${row.AverageTime}</td>
                        <td>${row.TotalTime}</td>
                        <td>${row.MaximumTime}</td>
                        <td>${row.ExampleQueryString || """"}</td>
                        <td>${row.ExampleBody || """"}</td>
                        <td><button onclick=""toggleDetails(${index})"">Details</button></td>
                    `;
                    tableBody.appendChild(mainRow);

                    const labels = [""Success"", ""Error"", ""This Week""];
                    const details = [
                        [row.SuccessRequestCount, row.SuccessAverageTime, row.SuccessTotalTime, row.SuccessMaximumTime],
                        [row.FailureRequestCount, row.FailureAverageTime, row.FailureTotalTime, row.FailureMaximumTime],
                        [row.ThisWeekRequestCount, row.ThisWeekAverageTime, row.ThisWeekTotalTime, row.ThisWeekMaximumTime],
                    ];

                    labels.forEach((label, i) => {
                        const detailsRow = document.createElement(""tr"");
                        detailsRow.classList.add(""hidden"", ""details-row"");
                        detailsRow.id = `details-${index}-${i}`;
                        detailsRow.innerHTML = `
                            <td colspan=""2"">${label}</td>
                            <td>${details[i][0]}</td>
                            <td>${details[i][1]}</td>
                            <td>${details[i][2]}</td>
                            <td>${details[i][3]}</td>
                            <td colspan=""3""></td>
                        `;
                        tableBody.appendChild(detailsRow);
                    });
                });

                totalsRow.innerHTML = `
                    <td></td>
                    <td>Total</td>
                    <td>${totalRequestCount}</td>
                    <td>${(totalAverageTime / totalRequestCount).toFixed(2)}</td>
                    <td>${totalTotalTime.toFixed(2)}</td>
                    <td>${totalMaxTime}</td>
                    <td colspan=""3""></td>
                `;
            }

            function toggleDetails(index) {
                const rows = document.querySelectorAll(`[id^=""details-${index}""]`);
                rows.forEach(row => row.classList.toggle(""hidden""));
            }

            function sortTable(column) {
                if (sortedColumn === column) {
                    sortDirection = sortDirection === ""asc"" ? ""desc"" : ""asc"";
                } else {
                    sortedColumn = column;
                    sortDirection = ""asc"";
                }

                const sortedData = [...dashboardData].sort((a, b) => {
                    const valA = a[column];
                    const valB = b[column];

                    if (valA < valB) return sortDirection === ""asc"" ? -1 : 1;
                    if (valA > valB) return sortDirection === ""asc"" ? 1 : -1;
                    return 0;
                });

                updateSortIndicators();
                renderTable(sortedData);
            }

            function updateSortIndicators() {
                document.querySelectorAll("".sort-indicator"").forEach(el => el.innerHTML = """");
                const activeHeader = document.querySelector(`th[data-column=""${sortedColumn}""] .sort-indicator`);
                activeHeader.innerHTML = sortDirection === ""asc"" ? ""▲"" : ""▼"";
            }

            document.querySelectorAll("".sortable"").forEach((header) => {
                header.addEventListener(""click"", () => {
                    const column = header.getAttribute(""data-column"");
                    sortTable(column);
                });
            });

            document.getElementById('search').addEventListener('input', function () {
                const searchValue = this.value.toLowerCase(); // Get the search input and convert to lowercase for case-insensitive search
                const filteredData = dashboardData.filter(row => 
                    row.RequestMethod.toLowerCase().includes(searchValue) || 
                    row.RequestPath.toLowerCase().includes(searchValue)
                );
                renderTable(filteredData); // Render filtered table
            });

            const sortedData = [...dashboardData].sort((a, b) => b.TotalTime - a.TotalTime); // Default sort
            renderTable(sortedData);
            updateSortIndicators();
        </script>";
        }

        private static string GetHtmlStyling()
        {
            return @"<style>
            body {
                font-family: Arial, sans-serif;
                margin: 20px;
                background-color: #f4f4f9;
            }

            h1 {
                text-align: left;
                font-size: 32px;
                color: #003366; /* Darker blue */
                border-bottom: 2px solid #003366;
                padding-bottom: 5px;
                margin-bottom: 20px;
            }

            .search-container {
                margin-bottom: 15px;
                text-align: right;
            }

            .search-container input {
                padding: 12px;
                width: 400px;
                font-size: 18px;
                border: 1px solid #ddd;
                border-radius: 4px;
            }

            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
                background: white;
            }

            th, td {
                padding: 10px;
                text-align: left;
                border: 1px solid #ddd;
            }

            th {
                background-color: #003366; /* Darker blue */
                color: white;
                cursor: pointer;
                user-select: none;
                position: relative;
            }

            th.sortable:hover {
                background-color: #002244;
            }

            th .sort-indicator {
                margin-left: 10px;
                font-size: 14px;
            }

            tr:hover {
                background-color: #f1f1f1;
            }

            .details-row {
                background-color: #e8f4fc;
                text-align: right;
            }

            .details-row td:first-child {
                text-align: left;
            }

            .total-row {
                background-color: #f0f8ff;
                font-weight: bold;
            }

            .hidden {
                display: none;
            }

            button {
                padding: 5px 10px;
                font-size: 14px;
                color: white;
                background-color: #003366;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            }

            button:hover {
                background-color: #002244;
            }

            button:active {
                transform: scale(0.95);
            }
        </style>";
        }
    }
}
