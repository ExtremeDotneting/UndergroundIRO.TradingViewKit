<template>
  <!-- TradingVueJs 101 (example from 'Getting Started' ) -->

  <trading-vue
    class="trading_view"
    ref="tradingVue"
    :data="chart"
    :width="this.width"
    :height="this.height"
  ></trading-vue>
</template>

<style lang="css">
@import "main.css";
</style>

<script>
import TradingVue from "trading-vue-js";
import Data from "../data/data.json";
import { constants } from "crypto";

export default {
  name: "app",
  components: { TradingVue },
  methods: {
    onResize(event) {
      this.width = window.innerWidth;
      this.height = window.innerHeight;
    }
  },
  mounted() {
    window.addEventListener("resize", this.onResize);
    window["TradingVueObj"] = this.$refs.tradingVue;
    this.$refs.tradingVue.titleTxt = "";
  },
  beforeDestroy() {
    window.removeEventListener("resize", this.onResize);
  },
  data() {
    window["TradingViewContext"] = {
      chart: Data,
      width: window.innerWidth,
      height: window.innerHeight
    };
    return window["TradingViewContext"];
  }
};
</script>
