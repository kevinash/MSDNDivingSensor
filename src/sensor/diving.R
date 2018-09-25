#
# " 1. Read the file"
# " 2. Parse the file, get data"
# " 3. Apply calculations for gyro & accelerometer columns"
# " 4. Get initial charts for gyro (w) & accelerometer (a)"
# " 5. Calculate flex and show charts
#
library(ggplot2)
library (dplyr)

ReadNewFormatFile <- function(df, debug_print = 0) {
  ##
  ## debug_print values: 0 - don't print, 1 - print totals, 2 - print all
  ## 
  first_comma_pos_next_line <- -1
  pos <- -1
  comma_cnt <- -1
  full_line <- "" # 
  parsed_set_line_i <- data.frame(T =as.POSIXct(character()), V2 = character(0))
  parsed_set <- data.frame(T =as.POSIXct(character()), V2 = character(0))
  
  if (debug_print > 0) {print (paste("Total rows =", nrow(df) ))}
  
  for(i in 1:nrow(df))
  {
    if (nchar(full_line) == 0) {
      #initial setup
      start <- 1
      full_line <- df$V4[i]
      parsed_set_line_i <- as.data.frame(rbind(c(df$V2[i], full_line)))
      
    } else{
      #count comma separators in full line
      comma_cnt <- sum(unlist(strsplit(full_line,"")) == ",")
      new_line_exists <- sum(unlist(strsplit(full_line,"")) == "\n")
      if (new_line_exists > 0) {pos <- regexpr(pattern ='\n',full_line)[1]}
      len_full_line <- nchar(full_line)
      first_comma_pos_next_line <- regexpr(pattern =',',df$V4[i])[1]
      
      # one line of data could be presented in one, 2 or 3 lines
      # when received '\n' - process line, or when next line has time fraction field(length = 8)
      if (
        (new_line_exists > 0  && pos == len_full_line) | 
        (first_comma_pos_next_line == 8 && len_full_line > 10) 
      ) 
      { 
        full_line <- gsub("\n", "", full_line)
        if (nchar(full_line)> 0) {
          parsed_set_line_i$V2 <- full_line                 #update full combined string
          parsed_set <- rbind(parsed_set,parsed_set_line_i) #save into resulting dataset
          
          #parse only full lines with 7 commas & 8 numbers - got all the columns
          if (comma_cnt==7) {
            s <- strsplit(full_line, ",")
            xx <- unique(unlist(s)[grepl('[A-Z]', unlist(s))])
            sap <- t(sapply(seq(s), function(i){
              wh <- which(!xx %in% s[[i]]); n <- suppressWarnings(as.numeric(s[[i]]))
              nn <- n[!is.na(n)]; if(length(wh)){ append(nn, NA, wh-1) } else { nn }
            })) 
            dt <- data.frame(parsed_set_line_i,sap)
            if (ncol(dt)==10) {
              #save data only when all columns are present, ignore lines with missing fields
              if (exists('dt_result') && is.data.frame(get('dt_result'))) {
                dt_result <-rbind(dt_result,dt) # need plyr to fill out missing values with NA
              }
              else{
                dt_result <- dt
              }
            }
          }
          if (debug_print==2 & (exists('dt_result') && is.data.frame(get('dt_result')))) {
            print(paste("SAVED: i=",i,"start=",start," full_line=",full_line,
                        " rows=",nrow(parsed_set)," parsed rows=",nrow(dt_result)))}
        }
        
        #reset values
        comma_cnt <- 0
        start <- 1
        pos <- -1
        
        full_line <- gsub(" ", "", df$V4[i]) 
        parsed_set_line_i <- as.data.frame(rbind(c(df$V2[i], full_line)))   #save next row in temp dataframe
        
      } else{
        #get next chunk, append and remove extra spaces
        full_line <- gsub(" ", "", paste(full_line, df$V4[i], sep="")) 
        start <- start + 1            
      }
    }
  }
  col_names <- c('t','line_to_parse',"t_fraction",'wx', 'wy', 'wz', 'ax', 'ay', 'az', 'o')
  colnames(dt_result) <- col_names            #rename columns
  
  if (debug_print>0) {print (paste("Rows read =", nrow(df), ", processed =", nrow(parsed_set), 
                                   ", parsed =", nrow(dt_result)))}
  return(dt_result)
}

PlotSingleChart <- function(gf,chart_title, mainColor, fileName, sidetext,
                            newPlot=FALSE, show_smooth = FALSE, timeAxis=NULL){
  g_range <- range(floor(min(gf, na.rm=T)), floor(max(gf,na.rm=T))+ 1)
  
  if (newPlot && !show_smooth) {
    if(missing(timeAxis) | is.null(timeAxis)) {
    plot(1:length(gf),gf,type="l",ylab="",xlab="",col=mainColor, lwd = 2 , ylim=g_range)
    } else {
      plot(timeAxis,gf,type="l",ylab="",xlab="",col=mainColor, lwd = 2 , ylim=g_range)
    }
    abline(h = 0, v = 100 * c(1:floor(length(gf) /100)), lty = 2, lwd = .2, col = "gray70")
  }else if (newPlot && show_smooth) {
    points =gf
    if(missing(timeAxis) | is.null(timeAxis)) {
      plot(points, ylim=c(min(gf, na.rm=T),max(gf, na.rm=T)),
           pch = 20, col = mainColor, cex = 1.5)
        lines(spline(1:length(gf) ,  points ), col=mainColor, lwd = 2)
    } else {
      plot(timeAxis,points, ylim=c(min(gf, na.rm=T),max(gf, na.rm=T)), 
           pch = 20, col = mainColor, cex = 1.5)
      lines(spline(timeAxis,  points ), col=mainColor, lwd = 2)
    }
    abline(h = 0, v = 100 * c(1:floor(length(gf) /100)), lty = 2, lwd = .2, col = "gray70")
  }else {
    lines(1:length(gf),gf,type="l",col=mainColor,lwd=2)
  }
  
  if (chart_title !="") {title(paste(chart_title,":",fileName), cex.main = 1,   col.main= "blue")}
  mtext(sidetext,side=4,col="blue",cex=1)
  
}

PlotInitialCharts <- function(df, fileName){
  ###############
  #
  # 4. Plot initial observations for Angular Velocity  & Accelleration 
  # 
  ###############
  par(mfcol = c(3,2),oma = c(2,2,0,0) + 0.1,mar = c(1, 1, 1, 1) + 0.5)
  
  #get column number for TimeSec 
  nTimeAxis <- which( colnames(df)=="t_sec" ) 
  
  PlotSingleChart(df$wx,"Angular Velocity ","red", fileName,"Wx",TRUE,FALSE,df[,nTimeAxis])
  PlotSingleChart(df$wy,"","green", fileName,"Wy",TRUE,FALSE,df[,nTimeAxis])
  PlotSingleChart(df$wz,"","blue", fileName,"Wz",TRUE,FALSE,df[,nTimeAxis])
  
  PlotSingleChart(df$ax,"Accelleration ","red", fileName,"Ax",TRUE,FALSE,df[,nTimeAxis])
  PlotSingleChart(df$ay,"","green", fileName,"Ay",TRUE,FALSE,df[,nTimeAxis])
  PlotSingleChart(df$az,"","blue", fileName,"Az",TRUE,FALSE,df[,nTimeAxis])

  par(mfcol = c(1,1),oma = c(2,2,0,0) + 0.1,mar = c(1, 1, 1, 1) + 0.5)
  p1<-ggplot() +
    geom_line(data = df, aes(x = df[,nTimeAxis], y = df$wx, colour = "Wx"), size = 1.4) + 
    geom_line(data = df, aes(x = df[,nTimeAxis], y = df$wy, color = "Wy"), size = 1.4) + 
    geom_line(data = df, aes(x = df[,nTimeAxis], y = df$wz, color = "Wz"), size = 1.4) + 
    scale_colour_manual("", 
                        breaks = c("Wx", "Wy", "Wz"),
                        values = c("red", "green", "blue")) +
    xlab("\nTime Fraction") + 
    ylab("Angular Velocity \n") + 
    ggtitle("Angular Velocity",paste("File: [",fileName,"]", sep="")) + 
    theme_bw() +
    theme(axis.text.x = element_text(size = 10, color = "black")) + 
    theme(plot.title = element_text(lineheight = .8, 
                                    face = "bold", colour = "black", 
                                    hjust = 0.5, vjust = 2, size = 10)) 
    #theme(panel.grid.major = element_blank(), panel.grid.minor = element_blank())
  p1
}

SlidingAvg<-function(y,lag,threshold,influence){
  
  filteredY <- y[0:lag]
  avgFilter <- NULL
  stdFilter <- NULL
  avgFilter[lag] <- mean(y[0:lag])
  stdFilter[lag] <- sd(y[0:lag])
  
  #1-st sliding average
  for (i in (lag+1):length(y)){
    if (abs(y[i]-avgFilter[i-1]) > threshold*stdFilter[i-1]) {
      filteredY[i] <- influence*y[i]+(1-influence)*filteredY[i-1]
    } else {
      filteredY[i] <- y[i]
    }
    avgFilter[i] <- mean(filteredY[(i-lag):i])
    stdFilter[i] <- sd(filteredY[(i-lag):i])
  }
  return(list("avgFilter"=avgFilter,"stdFilter"=stdFilter))
}

CalculateSpringBoardDivingValues <- function(sourceFile, lag =10, threshold = 2, influence = 1,
                                             debug_print = 0, 
                                             save_outputfiles = 0){
  
#SensorKit data
dat <- read.table(sourceFile, header = FALSE, skip=50,sep="\t",stringsAsFactors=FALSE)
dat2 <- dat[dat$V1=="A", ]                 #Get only A rows 
dat2$V4 <- gsub(" received", "", dat2$V3)  #remove Received
df <- ReadNewFormatFile(dat2, debug_print = debug_print)

# 3.values conversion 
#   wx, wy, wz *  0.001 * PI / 180.0 = radians/second (ang. Velocity)
#   ax, at, az = * 0.001 * 9.81 = m/s^2 (acceleration)
df$wx <- df$wx *  0.001 #ignore radians for now # * pi / 180.0
df$wy <- df$wy *  0.001 #ignore radians for now # * pi / 180.0
df$wz <- df$wz *  0.001 #ignore radians for now # * pi / 180.0
df$ax <- df$ax * 0.001 * 9.81
df$ay <- df$ay * 0.001 * 9.81
df$az <- df$az * 0.001 * 9.81
df$t_sec <- (df$t_fraction - df$t_fraction[1]) * 0.001 #get time in seconds from milliseconds, relative to the beginning of file

PlotInitialCharts (df, kitSensorFile)

# Find axis with the biggest values for Angular Velocity
maxWx <- max(abs(df$wx))
maxWy <- max(abs(df$wy))
maxWz <- max(abs(df$wz))

flexCol <- which( colnames(df)=="t_fraction" )       # values, we are interested in, are in columns after [t_fraction] 
flexCol <- flexCol + which.max(c(maxWx,maxWy,maxWz)) # take columns with biggest values
print (paste("We are using Max values of Angular Velocity by axis ",colnames(df[flexCol]),sep=""))
#========================================
# Calculating Flex with Angular Velocity
#========================================
#
# Get Date of the file
pos<-regexpr(pattern ='2018',basename(sourceFile))[[1]]
dt <-substr(basename(sourceFile),pos,pos+9) 

if (strptime(dt,"%Y-%m-%d")%>% is.na()) {             # tryFormats = c("%Y-%m-%d", "%Y/%m/%d","%Y%m%d")
  dt <-strptime(dt,"%Y%m%d")
} else {
  dt<-strptime(dt,"%Y-%m-%d")}

# Take only needed columns
# Use t_sec values [1:length(df)-1] as a Start time
# and values [2:length(df)] as End time of interval to calculate time delta

d<-data.frame("Start"=as.numeric(df$t_sec[1:length(df$t_sec)-1]), # index 1
              "End"=as.numeric(df$t_sec[2:length(df$t_sec)]),     # index start + 1
              "FlexValues"=df[1:length(df$t_fraction)-1, flexCol], # angular velocity around vertical axis
              Ax =  df[1:length(df$ax)-1, 7],
              Ay =  df[1:length(df$ay)-1, 8],
              Az =  df[1:length(df$az)-1, 9],
              stringsAsFactors=FALSE) 


# d$FlexValues  # Angular velocity we use to calculate max Flex 
# d$Az # linear accelleration along Z - vertical axis

#Smooth lines using moving average
aSm <- SlidingAvg(d$Az,lag,threshold,influence)
aSmMean <- abs(mean(aSm$avgFilter,na.rm=T))
minSmooth <- min(abs(abs(aSm$avgFilter) - aSmMean), na.rm = T) 
maxSmooth <-max(abs(abs(aSm$avgFilter) - aSmMean), na.rm = T)

deltaSmooth <- (maxSmooth - minSmooth) /5

# Looking for meaningful values, not just straight line 
st<- which(deltaSmooth < abs(abs(aSm$avgFilter) - aSmMean) & d$Start > 1)
posStart<-min(st) #d$Start[posStart] in seconds
posEnd<-max(st)   #d$Start[posEnd] in seconds
print (paste("Our Segment: start=",d$Start[posStart]," end=",d$Start[posEnd],sep=""))

par(mfcol = c(2,1),oma = c(2,2,0,0) + 0.1,mar = c(1,1,1,1) + 0.5)
#PlotSingleChart(d$Az,"Acceleration by Z(Az) ","blue", kitSensorFile,"Az",TRUE,FALSE,d$Start)

# The same charts for Angular Velocity
wSm <- SlidingAvg(d$FlexValues,lag,threshold,influence)

#Interested only in [posStart:posEnd]
# When displacement is at MAX, velocity passes 0 point 
revFirstMin <- which.min(rev(wSm$avgFilter[posStart:posEnd])) 
# after been max negative
posLastMin <- posEnd - revFirstMin +1                         
#Next index when Angular Velocity goes 
# through 0 is our starting integration point
nextZero <- min(which(floor(wSm$avgFilter[posLastMin:posEnd])>0)) 
# 0 position relative to Last Minimum 
nextZero <- nextZero + posLastMin -1                          
# Max position after 0 position
nextMaxAfterZero <- which.max(wSm$avgFilter[nextZero:posEnd])        
nextMaxAfterZero <- nextMaxAfterZero + nextZero -1
  
posMaxFlex <- nextZero
posHorizontal <-nextMaxAfterZero

PlotSingleChart(wSm$avgFilter[posStart:posEnd],"Angular Velocity Smooth OUR Segment ","cyan4", kitSensorFile,"Wx smooth deg.",TRUE, FALSE, d$Start[posStart:posEnd])
abline(v =  c(1:round(length(wSm$avgFilter))), lty = 2, lwd = .2, col = "gray70")
points(d$Start[posLastMin], wSm$avgFilter[posLastMin], col="cyan", pch=19, cex=1.25)
points(d$Start[c(posMaxFlex,posHorizontal)], wSm$avgFilter[c(posMaxFlex,posHorizontal)], col="red", pch=19, cex=1.25)

d$omega <-wSm$avgFilter

# Calculate delta t 
d$duration <-d$End -d$Start     # need delta time (duration) in seconds 
d$theta <- d$omega * d$duration # calculate change in angle over time

# Integrate Cumulative values for the Angle
d$thetaCum <-0.                      #start with 0 for all Cumulative Values of Angle
d$thetaCum[posMaxFlex:posHorizontal] <- cumsum(d$theta[posMaxFlex:posHorizontal])

d$theta2Cum <-0.                      #start with 0 for all Cumulative Values of Angle
d$theta2Cum[posLastMin:nextZero] <- cumsum(d$theta[posLastMin:nextZero])
d$theta3Cum <-0.
d$theta3Cum[posLastMin:posHorizontal] <- cumsum(d$theta[posLastMin:posHorizontal])

if (save_outputfiles !=0 ){
  outputFile <- sub('\\.txt', '', sourceFile) 
  outputFileOrig <- paste(outputFile,"_Origin.csv", sep="")
  outputFileFlex <- paste(outputFile,"_Flex.csv", sep="")
  write.csv(df, file=outputFileOrig, row.names=FALSE, quote=TRUE)
  write.csv(d, file=outputFileFlex, row.names=FALSE, quote=TRUE)
}

print (paste("We found: posLastMin=",d$Start[posLastMin],", posMaxFlex=",d$Start[posMaxFlex],", posHorizontal=",d$Start[posHorizontal],sep=""))

PlotSingleChart(2* d$thetaCum[posMaxFlex:posHorizontal],"Integrated Flex Angle (deg.) ","cyan4", kitSensorFile,"Theta",TRUE, TRUE, d$Start[posMaxFlex:posHorizontal]) 


timeTakeOff <- d$Start[posHorizontal] 
timeStart <- d$Start[posLastMin]   

writeLines (paste (
            "Hurdle landing time (Start)                         = ", timeStart,"\n",
            "Take Off time (End)                                 = ", timeTakeOff,"\n",
            "Board contact time (from hurdle landing to takeoff) = ",
            round(difftime(
            as.POSIXct(paste(dt,df$t[posHorizontal],sep=" ")),
            as.POSIXct(paste(dt,df$t[posLastMin],sep=" "))),digits = 4)," secs \n",
            "Maximum downward flexion of the board               = ", 
            round(max(abs(d$theta2Cum[posLastMin:nextZero])),digits=4)," deg.",
            " secs", sep="" ))
}



kitSensorDir <- "./data/"
kitSensorFile <- "82_Log2018-09-19_15_06_33.txt"
sourceFile <- paste(kitSensorDir, kitSensorFile, sep="")
lag       <- 5
threshold <- 3
influence <- 0.5

CalculateSpringBoardDivingValues (sourceFile,lag, threshold, influence,1)


