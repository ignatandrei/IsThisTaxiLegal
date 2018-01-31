if (!require("pacman")) install.packages("pacman")
pacman::p_load('XML','RSQLite', "magrittr", "RCurl", "rlist", "rvest", "pdftools", "dplyr", "devtools")
#if (!require(XML)) install.packages('XML')
#install.packages("magrittr")
#install.packages("RCurl")
#install.packages("rlist")
#install.packages("rvest")
library(stringr)
library(magrittr)
library(XML)
library(RCurl)
library(rlist)
library(rvest)
library(RSQLite)

getwd()

findLastTaxiPDFBucuresti <- function() {

    site <- "http://www.pmb.ro"
    # paste is concatenate strings !wtf!
    theurl <- paste(site, "/adrese_utile/transport_urban/autorizatii_taxi/autorizatii_TAXI.php", sep = "")
    #read the html
    content <- read_html(theurl)
    
    fnames <- html_nodes(x = content, xpath = '//a') %>% # find tags <a
                 html_attr("href") %>% # find href attribute
                .[grepl(glob2rx("*autorizatiilor*"), .)]  # glob2rx  - transform regular expression

    #print(head(fnames))
    
    pdfTaxi.url = paste(site, fnames[1], sep = "") # first url is the most recent
    
    
    return(pdfTaxi.url);# return need () !wtf!
}



downloadAndInterpretBucuresti <- function() {
pdfTaxi <- findLastTaxiPDFBucuresti()
pdfTaxi.local = "taxi.pdf" # how is the name of the local file downloaded
download.file(pdfTaxi.url, pdfTaxi.local, mode = "wb", cacheOK = F) #mode binary

#print(head(data))

getwd() # see current directory 
#pdfTaxi <-"taxi.pdf" #this for testing ,to not read html twice
data <- pdf_text(pdfTaxi) # read all text
#print(head(data))

#splitData <- unlist( lapply(data, function(x) {
    #strsplit(x, "\r\n")
#}))

splitData <- data %>%
            lapply(function(x) { strsplit(x, "\r\n") }) %>% #lapply transform apply into list
            unlist %>% # make a gigantic list
.[grepl("^[0-9]", .)] %>% # the items that are interesting begins with number (the  id)
.[grepl("[A-Z]", .)] # the items that are interesting contains names

splitData %>% head(10) %>% print # showing data
}
#print(head(splitData))
# many tries - do not enter
#write.table(data, "pdfTaxi.txt");
#read.table(text = pdftext, row.names = NULL)
#td <- content %>% html_nodes("pdf") %>% html_nodes("td")
#td
#fdata <- html_attr(fnames,name = "src")
#fdata
        #html_text()


#head(fnames)

#data <- content %>%
  #html_nodes(".pdf") %>%
  #html_attr("href")
  

#data
#cat(paste(data, collapse = "\n"))

#html <- getURL(theurl)
## parse html
#doc = htmlParse(html, asText = TRUE)

##plain.text <- xpathSApply(doc, "//a", xmlValue)
#r <- xmlRoot(doc)
#plain.data <- xmlGetAttr(r, "href")

#cat(paste(plain.data, collapse = "\n"))


#tables <- readHTMLTable(theurl)
#doc <- xmlTreeParse(tables)
#t <- getNodeSet(doc, "//a[@class='pdf']")

#testrun <- htmlTreeParse("http://www.pmb.ro/adrese_utile/transport_urban/autorizatii_taxi/autorizatii_TAXI.php", useInternalNodes = T)

#tables <- list.clean(tables, fun = is.null, recursive = FALSE)
#n <- unlist(lapply(tables, function(t) dim(t)[1]))
