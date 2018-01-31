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

findLastTaxiPDFCluj <- function() {

    site <- "http://www.primariaclujnapoca.ro"
    # paste is concatenate strings !wtf!
    theurl <- paste(site, "/informatii-publice/taximetrie.html", sep = "")
    #read the html
    content <- read_html(theurl)
    
    fnames <- html_nodes(x = content, xpath = '//a') %>% # find tags <a
                 html_attr("href") %>% # find href attribute
                .[grepl(glob2rx("*TAXI*"), .)]  # glob2rx  - transform regular expression

    #print(head(fnames))
    
    pdfTaxi.url = paste(site, fnames[1], sep = "") # first url is the most recent
    
    
    return(pdfTaxi.url);# return need () !wtf!
}

url <- findLastTaxiPDFCluj()
print(url)

