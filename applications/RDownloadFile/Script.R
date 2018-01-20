if (!require("pacman")) install.packages("pacman")
pacman::p_load('XML', "magrittr", "RCurl", "rlist", "rvest", "pdftools", "dplyr", "devtools")
#if (!require(XML)) install.packages('XML')
#install.packages("magrittr")
#install.packages("RCurl")
#install.packages("rlist")
#install.packages("rvest")
library(magrittr)
library(XML)
library(RCurl)
library(rlist)
library(rvest)
getwd()
site <-"http://www.pmb.ro"
theurl <- paste(site, "/adrese_utile/transport_urban/autorizatii_taxi/autorizatii_TAXI.php", sep = "")
content <- read_html(theurl)

fnames <- html_nodes(x = content, xpath = '//a') %>%
             html_attr("href") %>%
            .[grepl(glob2rx("*autorizatiilor*"), .)]

print(head(fnames))

pdfTaxi.url = paste(site, fnames[1], sep = "")
pdfTaxi.local="taxi.pdf"
download.file(pdfTaxi.url, pdfTaxi.local, mode = "wb", cacheOK = F)
data <- pdf_text(pdfTaxi.local)
print(head(data))

getwd()



#print(head(fada))
#cat(paste(head(fnames), collapse = "\n"))



#cat(paste(head(fdata), collapse = "\n"))
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
