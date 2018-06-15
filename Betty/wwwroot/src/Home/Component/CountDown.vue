<template>
    <v-layout align-center row>
        <v-flex>
            <span class="body-2 prefix">
                {{prefix}}
            </span>
        </v-flex>
        <v-layout column>
            <v-flex xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', 'display-1']">{{days}}</span>
                <span class="text">Ngày</span>
            </v-flex>
        </v-layout>
        <v-layout column >
            <v-flex xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', 'display-1']">{{hours}}</span>
                <span class="text">Giờ</span>
            </v-flex>
        </v-layout>
        <v-layout column>
            <v-flex xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', 'display-1']">{{minutes}}</span>
                <span class="text">Phút</span>
            </v-flex>
        </v-layout>
        <v-layout column>
            <v-flex xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', 'display-1']">{{seconds}}</span>
                <span class="text">Giây</span>
            </v-flex>
        </v-layout>
        
    </v-layout>
</template>
<script>
import parse from 'date-fns/parse'
export default {
    created: function () {
        if(this.date){
            this.dateTick = Math.trunc(parse(this.date) / 1000);
        }
        else{
            this.dateTick = 0;
        }
        this.tick();
        this.timerId = window.setInterval(this.tick, 1000);
    },
    props : {
        date : {
            type: String,
            required: true
        },
        prefix: {
            type: String,
            default: 'Countdown:'
        }
    },
    data() {
        return {
            now: Math.trunc((new Date()).getTime() / 1000),
            dateTick: null,
            timerId: null,
            timedOut: false
        }
    },
    methods: {
        tick: function(){
            let nowTick = Math.trunc((new Date()).getTime() / 1000);
            if(nowTick >= this.dateTick){
                this.$emit('timedout');
                this.timedOut = true;
                if(this.timerId)
                    window.clearInterval(this.timerId);
                return;
            }
            this.now = nowTick;
        },
        twoDigit: function(value){
            if(!value) return "00";
            if(value.toString().length <= 1) {
                return "0"+value.toString();
            }
            return value.toString();
        }
    },
    computed: {
        // dateNumber(){
        //     return Math.trunc(this.date / 1000)
        // },
        seconds() {
            return this.twoDigit(this.timedOut? 0 : (this.dateTick - this.now) % 60);
        },
        minutes() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.dateTick - this.now) / 60) % 60);
        },
        hours() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.dateTick - this.now) / 60 / 60) % 24);
        },
        days() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.dateTick - this.now) / 60 / 60 / 24));
        }
    }
}
</script>
<style scoped>
.text {
    color: #5dc596;
    font-weight: 400;
    text-align: center;
}
.digit {
    color: #E0E0E0;
    font-weight: 100;
    text-align: center;
}
.red-digit {
    color: #EF5350;
    font-weight: 100;
    text-align: center;
}
.prefix{
    color: #E0E0E0;
}
</style>