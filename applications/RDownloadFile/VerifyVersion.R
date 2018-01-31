if (!require("pacman")) install.packages("pacman")
pacman::p_load('XML', 'sqldf', 'RSQLite', "magrittr", "RCurl", "rlist", "rvest", "pdftools", "dplyr", "devtools")
if (!exists("findLastTaxiPDFBucuresti", mode = "function")) source("Bucuresti.R")
if (!exists("findLastTaxiPDFCluj", mode = "function")) source("Cluj.R")

nameFile <- paste(format(Sys.time(), "%Y%m%d"), "txt", sep = ".")
print(nameFile)
if (file.exists(nameFile)) file.remove(nameFile)
url <- findLastTaxiPDFBucuresti()

data <- data.frame("Bucuresti", url, stringsAsFactors = FALSE)
names(data) <- c("City", "Url")


url <- findLastTaxiPDFCluj()
newRow <- data.frame('Cluj', url)
names(newRow) <- c("City", "Url")
data <- rbind(data,newRow)
print(data)

#print(getwd())
#fileConn <- file(nameFile)
#writeLines(c(paste("Bucuresti", url)), fileConn)
#writeLines(c(paste("Bucuresti", url)), fileConn)
#close(fileConn)


